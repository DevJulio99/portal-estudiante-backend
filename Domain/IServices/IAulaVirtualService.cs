using MyPortalStudent.Domain.Dtos.AulaVirtual;

namespace MyPortalStudent.Domain.IServices
{
    public interface IAulaVirtualService
    {
        Task<SilaboResponseDto?> GetSilaboPorCursoAsync(SilaboRequestDto request);
    }
}