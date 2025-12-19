using MyPortalStudent.Domain.Dtos.AulaVirtual;

namespace MyPortalStudent.Domain.IServices
{
    public interface IAulaVirtualService
    {
        Task<SilaboResponseDto?> GetSilaboPorCursoAsync(SilaboRequestDto request);
        Task<MaterialesResponseDto?> GetMateriales(MaterialesRequestDto request);
        Task<RegistrarMaterialResponseDto?> RegistrarMaterial(RegistrarMaterialRequestDto request);
    }
}