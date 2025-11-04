using Microsoft.AspNetCore.Mvc;
using MyPortalStudent.Domain.IServices;
using Dapper;
using Npgsql;
using MyPortalStudent.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace MyPortalStudent.Controllers
{
    [ApiController]
    [Route("api/test/tenant")]
    public class TenantTestController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantService _tenantService;
        private readonly ILogger<TenantTestController> _logger;

        public TenantTestController(
            IConfiguration configuration,
            ITenantService tenantService,
            ILogger<TenantTestController> logger)
        {
            _configuration = configuration;
            _tenantService = tenantService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint de prueba para verificar el tenant actual
        /// </summary>
        [HttpGet("current")]
        public IActionResult GetCurrentTenant()
        {
            var tenant = _tenantService.GetCurrentTenant();
            return Ok(new
            {
                success = true,
                currentTenant = tenant ?? "NO ESTABLECIDO",
                message = tenant != null 
                    ? $"El tenant actual es: {tenant}" 
                    : "No se ha establecido ningún tenant. Asegúrate de enviar un JWT token válido con Codigo_Sede en el header Authorization."
            });
        }

        /// <summary>
        /// Endpoint para verificar el tenant establecido en la base de datos
        /// </summary>
        [HttpGet("database")]
        public async Task<IActionResult> GetDatabaseTenant()
        {
            try
            {
                string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                var tenantContext = _tenantService.GetCurrentTenant();
                _logger.LogInformation("[MULTITENANT] Tenant en contexto antes de establecer en BD: {Tenant}", tenantContext ?? "NULL");

                // Establecer el tenant si está disponible
                await connection.SetTenantIfAvailableAsync(_tenantService, _logger);

                // Verificar el tenant en la base de datos
                var tenantDb = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT current_setting('app.current_tenant', true)");

                _logger.LogInformation("[MULTITENANT] Tenant en BD después de establecer: {Tenant}", tenantDb ?? "NULL");

                return Ok(new
                {
                    success = true,
                    tenantInContext = tenantContext ?? "NO ESTABLECIDO",
                    tenantInDatabase = tenantDb ?? "NO ESTABLECIDO",
                    match = tenantContext == tenantDb,
                    message = tenantDb != null && !string.IsNullOrWhiteSpace(tenantDb)
                        ? $"Tenant en BD: {tenantDb}, Tenant en Context: {tenantContext ?? "NO ESTABLECIDO"}"
                        : "No se pudo obtener el tenant de la base de datos"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar tenant en base de datos");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Endpoint para verificar que las políticas RLS funcionan
        /// </summary>
        [HttpGet("rls-test")]
        public async Task<IActionResult> TestRowLevelSecurity()
        {
            try
            {
                string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
                await using var connection = await GetConnectionWithTenantAsync();

                // Verificar el tenant ANTES de hacer la consulta
                var tenantBeforeQuery = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT current_setting('app.current_tenant', true)");
                _logger.LogInformation("[MULTITENANT RLS TEST] Tenant antes de consulta: {Tenant}", tenantBeforeQuery ?? "NULL");

                var tenantInContext = _tenantService.GetCurrentTenant();
                _logger.LogInformation("[MULTITENANT RLS TEST] Tenant en contexto: {Tenant}", tenantInContext ?? "NULL");

                // Verificar usuario actual y permisos
                var userInfo = await connection.QueryFirstOrDefaultAsync<(
                    string current_user,
                    string session_user,
                    bool rolbypassrls
                )?>("SELECT current_user, session_user, rolbypassrls FROM pg_roles WHERE rolname = current_user");
                var userName = userInfo?.current_user ?? "unknown";
                var bypassRls = userInfo?.rolbypassrls ?? false;
                _logger.LogInformation("[MULTITENANT RLS TEST] Usuario BD: {User}, BYPASSRLS: {BypassRls}", 
                    userName, bypassRls);

                // Verificar si RLS está habilitado en la tabla
                var rlsEnabled = await connection.QueryFirstOrDefaultAsync<bool>(
                    "SELECT rowsecurity FROM pg_tables WHERE schemaname = 'public' AND tablename = 'alumno'");
                _logger.LogInformation("[MULTITENANT RLS TEST] RLS habilitado en tabla alumno: {Enabled}", rlsEnabled);

                // Intentar contar alumnos (debe filtrar por tenant automáticamente)
                var countAlumnos = await connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(*) FROM public.alumno");
                _logger.LogInformation("[MULTITENANT RLS TEST] Conteo total de alumnos: {Count}", countAlumnos);

                // Contar manualmente con la condición
                var countManual = await connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(*) FROM public.alumno WHERE (codigo_sede)::text = current_setting('app.current_tenant', true)");
                _logger.LogInformation("[MULTITENANT RLS TEST] Conteo manual con condición: {Count}", countManual);

                // Verificar el tenant DESPUÉS de hacer la consulta
                var currentTenant = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT current_setting('app.current_tenant', true)");

                // Obtener distribución por sede
                var distribucionPorSede = await connection.QueryAsync<dynamic>(
                    "SELECT codigo_sede, COUNT(*) as cantidad FROM public.alumno GROUP BY codigo_sede ORDER BY codigo_sede");

                return Ok(new
                {
                    success = true,
                    tenantInContext = tenantInContext ?? "NO ESTABLECIDO",
                    tenantBeforeQuery = tenantBeforeQuery ?? "NO ESTABLECIDO",
                    currentTenant = currentTenant ?? "NO ESTABLECIDO",
                    alumnosCount = countAlumnos,
                    alumnosCountManual = countManual,
                    rlsEnabled = rlsEnabled,
                    currentUser = new
                    {
                        user = userName,
                        bypassRls = bypassRls.ToString()
                    },
                    distribucionPorSede = distribucionPorSede.Select(d => new { codigo_sede = d.codigo_sede, cantidad = d.cantidad }),
                    message = $"Con tenant '{currentTenant}', se encontraron {countAlumnos} alumnos (manual: {countManual}). " +
                              "Si el usuario tiene BYPASSRLS=true, las políticas RLS no se aplican. " +
                              "Este conteo debería ser solo de alumnos de la sede correspondiente."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al probar RLS");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    message = "Si el error menciona 'no existe', puede ser que el tenant no se haya establecido correctamente."
                });
            }
        }

        /// <summary>
        /// Endpoint de diagnóstico para verificar el contenido del token JWT
        /// </summary>
        [HttpGet("debug-token")]
        public IActionResult DebugToken()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(new
                    {
                        success = false,
                        message = "No se encontró el header Authorization con formato Bearer token",
                        hasAuthHeader = !string.IsNullOrWhiteSpace(authHeader),
                        authHeaderFormat = authHeader != null ? authHeader.Substring(0, Math.Min(20, authHeader.Length)) : ""
                    });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                
                if (!handler.CanReadToken(token))
                {
                    return Ok(new
                    {
                        success = false,
                        message = "No se puede leer el token JWT",
                        tokenLength = token.Length
                    });
                }

                var jsonToken = handler.ReadJwtToken(token);
                var claims = jsonToken.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList();
                
                var codigoSede = jsonToken.Claims.FirstOrDefault(c => c.Type == "Codigo_Sede")?.Value;
                var tenant = _tenantService.GetCurrentTenant();

                return Ok(new
                {
                    success = true,
                    message = "Token decodificado correctamente",
                    tokenInfo = new
                    {
                        issuer = jsonToken.Issuer,
                        audience = jsonToken.Audiences?.FirstOrDefault(),
                        validFrom = jsonToken.ValidFrom,
                        validTo = jsonToken.ValidTo,
                        isExpired = jsonToken.ValidTo < DateTime.UtcNow
                    },
                    claims = claims,
                    codigoSedeInToken = codigoSede ?? "NO ENCONTRADO O VACÍO",
                    tenantInContext = tenant ?? "NO ESTABLECIDO",
                    match = codigoSede == tenant
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al decodificar token");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Endpoint de diagnóstico para probar la función set_tenant directamente
        /// </summary>
        [HttpGet("test-set-tenant")]
        public async Task<IActionResult> TestSetTenantDirectly()
        {
            try
            {
                string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                var tenantContext = _tenantService.GetCurrentTenant();
                
                // Obtener la definición de la función
                var functionDefinition = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT pg_get_functiondef(p.oid) FROM pg_proc p JOIN pg_namespace n ON p.pronamespace = n.oid WHERE n.nspname = 'app' AND p.proname = 'set_tenant'");
                
                // Verificar que la función existe
                var functionExists = await connection.QueryFirstOrDefaultAsync<bool>(
                    "SELECT EXISTS(SELECT 1 FROM pg_proc p JOIN pg_namespace n ON p.pronamespace = n.oid WHERE n.nspname = 'app' AND p.proname = 'set_tenant')");

                if (!functionExists)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "La función app.set_tenant no existe en la base de datos",
                        tenantInContext = tenantContext ?? "NO ESTABLECIDO"
                    });
                }

                // Intentar establecer el tenant directamente
                if (!string.IsNullOrWhiteSpace(tenantContext))
                {
                    // Verificar primero si el código de sede existe en la tabla sede
                    var sedeExists = await connection.QueryFirstOrDefaultAsync<bool>(
                        "SELECT EXISTS(SELECT 1 FROM public.sede WHERE codigo_sede = @codigo_sede)",
                        new { codigo_sede = tenantContext });

                    // Probar con SELECT app.set_tenant
                    int result1 = -1;
                    string? tenantAfter1 = null;
                    string? errorMessage = null;
                    
                    try
                    {
                        result1 = await connection.ExecuteAsync(
                            "SELECT app.set_tenant(@codigo_sede)",
                            new { codigo_sede = tenantContext });

                        tenantAfter1 = await connection.QueryFirstOrDefaultAsync<string>(
                            "SELECT current_setting('app.current_tenant', true)");
                    }
                    catch (Exception exFunc)
                    {
                        errorMessage = exFunc.Message;
                        _logger.LogError(exFunc, "Error al ejecutar app.set_tenant");
                    }

                    // Probar con PERFORM app.set_tenant (si retorna void)
                    try
                    {
                        await connection.ExecuteAsync(
                            "PERFORM app.set_tenant(@codigo_sede)",
                            new { codigo_sede = tenantContext });
                        var tenantAfter2 = await connection.QueryFirstOrDefaultAsync<string>(
                            "SELECT current_setting('app.current_tenant', true)");
                    }
                    catch (Exception ex2)
                    {
                        _logger.LogWarning(ex2, "PERFORM falló, intentando otro método");
                    }

                    // Probar estableciendo directamente con SET
                    try
                    {
                        await connection.ExecuteAsync(
                            "SET LOCAL app.current_tenant = @codigo_sede",
                            new { codigo_sede = tenantContext });
                        var tenantAfter3 = await connection.QueryFirstOrDefaultAsync<string>(
                            "SELECT current_setting('app.current_tenant', true)");
                    }
                    catch (Exception ex3)
                    {
                        _logger.LogWarning(ex3, "SET LOCAL falló");
                    }

                    // Verificar el valor actual
                    var tenantFinal = await connection.QueryFirstOrDefaultAsync<string>(
                        "SELECT current_setting('app.current_tenant', true)");

                    // Verificar si la variable existe pero está vacía
                    var tenantExists = await connection.QueryFirstOrDefaultAsync<bool>(
                        "SELECT EXISTS(SELECT 1 FROM pg_settings WHERE name = 'app.current_tenant')");

                    return Ok(new
                    {
                        success = true,
                        tenantInContext = tenantContext,
                        functionExists = true,
                        functionDefinition = functionDefinition ?? "NO ENCONTRADA",
                        sedeExistsInTable = sedeExists,
                        setTenantResult = result1,
                        errorMessage = errorMessage,
                        tenantAfterSelect = tenantAfter1 ?? "NULL o no existe",
                        tenantFinal = tenantFinal ?? "NULL o no existe",
                        tenantVariableExists = tenantExists,
                        message = $"Función existe: {functionExists}, Sede existe en tabla: {sedeExists}, Resultado de set_tenant: {result1}, Error: {errorMessage ?? "ninguno"}, Tenant después: '{tenantAfter1 ?? "NULL"}'"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        message = "No hay tenant en el contexto para establecer",
                        tenantInContext = "NO ESTABLECIDO",
                        functionExists = functionExists,
                        functionDefinition = functionDefinition ?? "NO ENCONTRADA"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al probar set_tenant directamente");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        private async Task<NpgsqlConnection> GetConnectionWithTenantAsync()
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            await connection.SetTenantIfAvailableAsync(_tenantService, _logger);
            return connection;
        }
    }
}

