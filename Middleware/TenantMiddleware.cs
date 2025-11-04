using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MyPortalStudent.Domain.IServices;
using System.Linq;

namespace MyPortalStudent.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantMiddleware> _logger;

        public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
        {
            // Omitir el middleware para:
            // 1. Preflight requests (OPTIONS) - necesarios para CORS
            // 2. Rutas de autenticación que no requieren tenant
            var path = context.Request.Path.Value?.ToLower() ?? "";
            
            if (context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                // Permitir preflight requests de CORS sin verificar tenant
                await _next(context);
                return;
            }
            
            if (path.Contains("/api/auth/login") || 
                path.Contains("/api/auth/generar-captcha") || 
                path.Contains("/api/auth/validar-captcha") ||
                path.Contains("/api/auth/refresh-token") ||
                path.Contains("/swagger"))
            {
                await _next(context);
                return;
            }

            string? codigoSede = null;

            // Primero intentar obtener el código de sede del User principal (si la autenticación está configurada)
            codigoSede = context.User?.FindFirstValue("Codigo_Sede");
            _logger.LogDebug("[MULTITENANT] Codigo_Sede desde context.User: {CodigoSede}", codigoSede ?? "null o vacío");

            // Si no está disponible, intentar extraer del token JWT del header Authorization
            if (string.IsNullOrWhiteSpace(codigoSede))
            {
                var tokenInfo = ExtractTokenAndSetUser(context);
                codigoSede = tokenInfo.CodigoSede;
                
                _logger.LogDebug("[MULTITENANT] Codigo_Sede desde ExtractTokenAndSetUser: {CodigoSede}", codigoSede ?? "null o vacío");
            }

            if (string.IsNullOrWhiteSpace(codigoSede))
            {
                // Si no hay código de sede (token inválido o ausente), bloquear la request
                _logger.LogWarning("[MULTITENANT] No se pudo obtener Codigo_Sede. Ruta: {Path}, Method: {Method}. Bloqueando request.", 
                    path, context.Request.Method);
                
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "Se requiere autenticación. Por favor, proporcione un token JWT válido en el header Authorization.",
                    error = "Unauthorized"
                });
                return;
            }

            try
            {
                // Establecer el tenant en HttpContext (se establecerá en cada conexión automáticamente)
                await tenantService.SetTenantAsync(codigoSede);
                _logger.LogInformation("[MULTITENANT] Tenant establecido en HttpContext: {CodigoSede} para ruta: {Path}", 
                    codigoSede, path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al almacenar el tenant: {CodigoSede}", codigoSede);
                // Si hay error al establecer el tenant, bloquear la request por seguridad
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "Error al procesar la autenticación. Por favor, contacte al administrador.",
                    error = "InternalServerError"
                });
                return;
            }

            await _next(context);
        }

        /// <summary>
        /// Extrae el token JWT, establece el User principal con los claims, y retorna el código de sede
        /// </summary>
        private (string? CodigoSede, bool TokenValid) ExtractTokenAndSetUser(HttpContext context)
        {
            try
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authHeader))
                {
                    _logger.LogDebug("[MULTITENANT] No se encontró header Authorization");
                    return (null, false);
                }

                if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogDebug("[MULTITENANT] Header Authorization no comienza con 'Bearer '");
                    return (null, false);
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogDebug("[MULTITENANT] Token está vacío después de 'Bearer '");
                    return (null, false);
                }

                // Decodificar el token JWT sin validar (solo para extraer claims)
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(token))
                {
                    _logger.LogWarning("[MULTITENANT] No se puede leer el token JWT");
                    return (null, false);
                }

                var jsonToken = handler.ReadJwtToken(token);
                
                // Establecer el User principal con los claims del token para que las validaciones de autorización funcionen
                var claimsIdentity = new ClaimsIdentity(jsonToken.Claims, "jwt");
                context.User = new ClaimsPrincipal(claimsIdentity);
                
                // Log todos los claims para debugging
                var allClaims = jsonToken.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
                _logger.LogDebug("[MULTITENANT] Claims en el token: {Claims}", string.Join(", ", allClaims));
                
                var codigoSedeClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "Codigo_Sede");
                var codigoSedeValue = codigoSedeClaim?.Value;
                
                if (codigoSedeClaim == null)
                {
                    _logger.LogWarning("[MULTITENANT] No se encontró el claim 'Codigo_Sede' en el token");
                }
                else if (string.IsNullOrWhiteSpace(codigoSedeValue))
                {
                    _logger.LogWarning("[MULTITENANT] El claim 'Codigo_Sede' existe pero está vacío o es null");
                }
                
                return (string.IsNullOrWhiteSpace(codigoSedeValue) ? null : codigoSedeValue, true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[MULTITENANT] Error al extraer código de sede del token JWT");
                return (null, false);
            }
        }
    }
}

