using MyPortalStudent.Domain.IServices;

namespace MyPortalStudent.Services
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task SetTenantAsync(string codigoSede)
        {
            if (string.IsNullOrWhiteSpace(codigoSede))
            {
                throw new ArgumentException("El código de sede no puede ser nulo o vacío.", nameof(codigoSede));
            }

            // Almacenar el tenant en HttpContext
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Items["CurrentTenant"] = codigoSede;
            }

            return Task.CompletedTask;
        }

        public string? GetCurrentTenant()
        {
            return _httpContextAccessor.HttpContext?.Items.TryGetValue("CurrentTenant", out var tenant) == true
                ? tenant?.ToString()
                : null;
        }
    }
}

