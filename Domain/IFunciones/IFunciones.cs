namespace MyPortalStudent.Domain.Ifunciones {
public interface IFuncionesApi
    {
        Task<List<AlumnoDTO>> getAlumnos();
        Task<List<PerfilDTO>> getAlumnosId(int idAlum);
        Task<List<HorarioResponse>> getHorarioId(int idAlum, string fechaInicio, string fechaFin);

        Task<List<CursoDTO>> getCursos(int idAlum);
    }
}