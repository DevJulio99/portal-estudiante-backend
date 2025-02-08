using MyPortalStudent.Domain.DTOs.CompetenciasGenerales;

namespace MyPortalStudent.Domain.IServices
{
    public interface ICompetenciasGeneralesService
    {
        // Task UploadFile(CompetenciasGeneralesUploadFileDTO uploadFileDTO);
        Task<Boolean> ExamenAleatorio(GenerarExamenDTO request);
        Task<List<ExamenDTO>> listarExamen(int idPostulante, int idCompetencia);
        Task<List<CompetenciaDTO>> listarCompetencias();
        Task<Boolean> ActualizarRespuesta(RespuestaReq request);
        Task<Boolean> CompetenciaCompleta(int idPostulante, int idCompetencia);
        Task<Boolean> registrarPostulante(RegistrarPostulanteDTO postulanteDto);
        Task<List<PostulanteDTO>> listarPostulante(string? dniPostulante);
        Task<Boolean> registrarActividadPostulante(UltimaActividadDTO ultimaActividadDto);
        Task<List<UltimaRespuestaExamenDTO>> getUltimaRespuestaExamen(int idPostulante, int idCompetencia);
        //Task<string> getUltimaActividadPostulante(int idPostulante, int idCompetencia);
        Task<List<ListaEstadoCompetenciaDTO>> listarEstadoCompetencia(int idPostulante, int? idCompetencia);
        Task<Boolean> registrarEstadoCompetencia(EstadoCompetenciaDTO estadoCompetenciaDto);
        Task<Boolean> actualizarEstadoCompetencia(EstadoCompetenciaDTO estadoCompetenciaDto);
        Task<Boolean> alumnoHabilitado(string? dniAlumno);
    }
}
