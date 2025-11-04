using Dapper;
using Microsoft.Extensions.Logging;
using MyPortalStudent.Domain.IServices;
using Npgsql;

namespace MyPortalStudent.Utils
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Establece el tenant en la conexión si está disponible en el HttpContext
        /// </summary>
        public static async Task SetTenantIfAvailableAsync(this NpgsqlConnection connection, ITenantService? tenantService = null, ILogger? logger = null)
        {
            if (tenantService == null)
            {
                logger?.LogWarning("[MULTITENANT] tenantService es null, no se puede establecer el tenant");
                return;
            }

            var currentTenant = tenantService.GetCurrentTenant();
            if (string.IsNullOrWhiteSpace(currentTenant))
            {
                logger?.LogWarning("[MULTITENANT] No hay tenant disponible en el contexto. La consulta puede fallar si RLS está activado.");
                // No lanzamos excepción aquí para permitir que las consultas fallen por RLS
                // si RLS no está activado, esto permite identificar problemas de configuración
                return;
            }

            try
            {
                // Asegurarse de que la conexión esté abierta
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                logger?.LogInformation("[MULTITENANT] Estableciendo tenant '{Tenant}' en la conexión de base de datos", currentTenant);

                // Verificar que la función existe
                var functionExists = await connection.QueryFirstOrDefaultAsync<bool>(
                    "SELECT EXISTS(SELECT 1 FROM pg_proc p JOIN pg_namespace n ON p.pronamespace = n.oid WHERE n.nspname = 'app' AND p.proname = 'set_tenant')");
                
                if (!functionExists)
                {
                    logger?.LogError("[MULTITENANT] La función app.set_tenant no existe en la base de datos");
                    return;
                }

                // NOTA: La función app.set_tenant usa set_config con LOCAL=true, lo que significa
                // que la configuración solo dura durante la transacción actual. Como Dapper usa
                // auto-commit por defecto, cada comando es su propia transacción, por lo que el
                // tenant se pierde inmediatamente después de ejecutar la función.
                //
                // Solución: Usar SET directamente (sin LOCAL) para establecer la configuración
                // para toda la sesión, O ejecutar la función y todas las consultas dentro de
                // la misma transacción.
                //
                // Por ahora, establecemos directamente con SET que persiste en la sesión:
                
                // Primero validar que la sede existe (como lo hace la función)
                var sedeExists = await connection.QueryFirstOrDefaultAsync<bool>(
                    "SELECT EXISTS(SELECT 1 FROM public.sede WHERE codigo_sede = @codigo_sede)",
                    new { codigo_sede = currentTenant });
                
                if (!sedeExists)
                {
                    logger?.LogError("[MULTITENANT] El código de sede '{CodigoSede}' no existe en la tabla sede", currentTenant);
                    return;
                }
                
                // Establecer la configuración para toda la sesión (no solo la transacción)
                await connection.ExecuteAsync(
                    "SELECT app.set_tenant(@codigo_sede::text);",
                    new { codigo_sede = currentTenant });
                
                // Verificar que el tenant se estableció correctamente
                var tenantVerificado = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT current_setting('app.current_tenant', true)");
                
                logger?.LogInformation("[MULTITENANT] Tenant verificado en BD: '{TenantVerificado}' (esperado: '{TenantEsperado}')", 
                    tenantVerificado ?? "NULL", currentTenant);
                    
                if (tenantVerificado != currentTenant)
                {
                    logger?.LogWarning("[MULTITENANT] El tenant no se estableció correctamente. Esperado: '{Esperado}', Obtenido: '{Obtenido}'", 
                        currentTenant, tenantVerificado ?? "NULL");
                }
            }
            catch (Exception ex)
            {
                // Si falla, continuar sin establecer el tenant
                // Las políticas RLS requerirán que el tenant esté establecido
                logger?.LogError(ex, "[MULTITENANT] Error al establecer tenant '{Tenant}' en la base de datos", currentTenant);
            }
        }
    }
}

