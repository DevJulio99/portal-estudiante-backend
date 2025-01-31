namespace MyPortalStudent.Domain.Ifunciones {
public interface IFuncionesApi
    {
        Task<List<AlumnoDTO>> getAlumnos();
        Task<List<PerfilDTO>> getAlumnosId(int idAlum);
        Task<List<HorarioResponse>> getHorarioId(int idAlum, string fechaInicio, string fechaFin);

        Task<List<CursoDTO>> getCursos(int idAlum);
        Task<List<ReporteMatriculaColegioDTO>> getCursosColegio(int idAlum, int anio);
        Task<List<AlumnoAsistenciaDTO>> getAsistenciasAlumno(int idAlum, string bimester, string codCurso, int anio);
        Task<List<HorarioxAulaDTO>> getHorariosxAula(int idAula);
        Task<List<HorarioCursoxAlumnnoDTO>> getHorariosCursoxAlumno(int idAlumno);
        Task<List<HorarioCursoxDocenteDTO>> getHorarioCursoxDocente(int idDocente);
        Task<List<NotasxBimestreDTO>> getNotasxBimestre(int idAlum, string tipoPeriodo, int anio);
        Task<List<PagoDTO>> getPagosPorAlumno(int idAlumno, int anio);
        Task<List<CalendarioAcademicoDTO>> GetCalendarioAcademico(int anio);

    }
}