using MyPortalStudent.Domain.DTOs;

namespace MyPortalStudent.Domain.IServices
{
    public interface IMatriculaService
    {
        Task<MatriculaResponseDTO> RealizarMatricula(MatriculaRegistrarDTO matriculaDto);
        Task<List<MatriculaListarDTO>> ListarMatriculasPorPeriodo(int idPeriodo, string? codigoSede = null);
        Task<MatriculaDTO?> ObtenerMatriculaPorId(int idMatricula);
        Task<List<MatriculaDTO>> ObtenerMatriculasPorAlumno(int idAlumno);
        Task<bool> VerificarMatriculaAlumno(int idAlumno, int idPeriodo);
        Task<bool> ActualizarEstadoMatricula(int idMatricula, string nuevoEstado);
        Task<bool> DesactivarMatricula(int idMatricula);
        Task<List<MatriculaDTO>> ObtenerMatriculasActivasPorSede(string codigoSede);
        Task<List<PeriodoAcademicoDTO>> ListarPeriodosDisponiblesParaMatricula(string codigoSede);
        Task<List<CursoSeccionDTO>> GetCursosPorGrado(int idGrado, string tipoInstitucion);
    }
}