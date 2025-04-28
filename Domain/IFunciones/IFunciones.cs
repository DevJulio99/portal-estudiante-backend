namespace MyPortalStudent.Domain.Ifunciones {
public interface IFuncionesApi
    {
        Task<List<AlumnoDTO>> getAlumnos();
        Task<List<PerfilDTO>> getAlumnosId(string? numDocUsuario);
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
        Task<List<CategoriaDocumentoDTO>> GetDocumentosConCategoria();
        Task<List<EventoDTO>> GetEventos();
        Task<List<UbicacionEventoDTO>> GetUbicacionesEvento(int eventoId);
        Task<List<ObligacionPorPeriodoDTO>> GetObligacionesPagadas(int idAlumno);
        Task<Boolean> setImagenPago(ImagenPagoDto imagenPagoDto);
        Task<List<PagoDTO>> getPagosPorSede(SedePaginadoDTO sedePaginadoDto);
        Task<List<AlumnoDTO>> getAlumnoPorSede(SedePaginadoDTO listaAlumno);
        Task<List<AlumnoDTO>> filtrarAlumno(FiltroAlumnoDTO filtroAlumno);
        Task<Boolean> registrarUsuarioAlumno(AlumnoRegistrarDTO alumnoRegistrarDto);
        Task<Boolean> actualizarUsuarioAlumno(AlumnoRegistrarDTO alumnoRegistrarDto);
        Task<Boolean> eliminarUsuarioAlumno(string numeroDocumento);
        Task<Boolean> AddDocument(DocumentoAddDTO documentoAddDto);
        Task<List<GradoDTO>> GetGrados();
        Task<List<CursoListarDTO>> ListarCursosPorSede(SedePaginadoDTO listaCurso);
        Task<List<CursoListarDTO>> FiltrarCurso(FiltroCursoDTO filtroCurso);
        Task<Boolean> RegistrarCurso(CursoRegistrarDTO cursoRegistrarDto);
        Task<Boolean> ActualizarCurso(CursoActualizarDTO cursoActualizarDto);
        Task<Boolean> EliminarCurso(int idCurso);
    }
}