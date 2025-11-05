using MyPortalStudent.Domain.DTOs;
using MyPortalStudent.Domain.DTOs.Notas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyPortalStudent.Domain.IServices
{
    public interface INotasService
    {
        Task<List<PeriodoSedeDTO>> GetPeriodoPorSede(PeriodoRequestDTO request);
        Task<List<GradoSedeDTO>> GetGradoPorSede(GradoRequestDTO request);
        Task<List<SubperiodoDTO>> GetSubperiodosPorPeriodo(SubperiodoRequestDTO request);
        Task<List<SeccionGradoDTO>> GetSeccionesPorGrado(SeccionesRequestDTO request);
        Task<List<CursoGradoDTO>> GetCursosPorGrado(CursosPorGradoSedeRequestDTO request);
        Task<List<AlumnoFiltroDTO>> GetAlumnosPorFiltro(AlumnosPorFiltroRequestDTO request);
        Task<List<NotasAlumnoDTO>> GetNotasAlumno(NotasAlumnoRequestDTO request);
        Task<BaseResponseDTO> GestionarNotasAlumno(GestionarNotaDto request);
    }
}