namespace MyPortalStudent.Domain.Ifunciones {
public interface IFuncionesApi
    {
        Task<List<AlumnoDTO>> getAlumnos();
        Task<List<AlumnoDTO>> getAlumnosId(int idAlum);
    }
}