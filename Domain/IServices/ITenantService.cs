namespace MyPortalStudent.Domain.IServices
{
    public interface ITenantService
    {
        Task SetTenantAsync(string codigoSede);
        string? GetCurrentTenant();
    }
}

