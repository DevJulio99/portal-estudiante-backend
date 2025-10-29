using System.Security.Permissions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyPortalStudent.Domain;
using MyPortalStudent.Domain.Ifunciones;

namespace MyPortalStudent.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class PortalController : ControllerBase
    {
        private readonly IFuncionesApi _funcionesApi;

        public PortalController(IFuncionesApi funcionesApi)
        {
            this._funcionesApi = funcionesApi;
        }

        [HttpGet("Alumnos")]
        public async Task<ActionResult> GetAlumnos()
        {
            var data = await _funcionesApi.getAlumnos();
            var apiResult = new ApiResponse<List<AlumnoDTO>>{ Data = data };
            return Ok(apiResult);
        }

        [HttpGet("AlumnosxId/{numDocUsuario}")]
        public async Task<ActionResult> GetAlumnosId(string? numDocUsuario)
        {
            var data = await _funcionesApi.getAlumnosId(numDocUsuario);
            if (data.Count == 0)
            {
                return NotFound(new ApiResponse<List<PerfilDTO>> { Success = false, Message = "No se encontro alumno", Data = [] });
            }
            var apiResult = new ApiResponse<List<PerfilDTO>> { Success = true, Message = "Se encontro alumno", Data = data };
            return Ok(apiResult);
        }

        [HttpGet("HorarioxId/{idAlum}/{fechaInicio}/{fechaFin}")]
        public async Task<ActionResult> GetHorarioId(int idAlum, string fechaInicio, string fechaFin)
        {
            var data = await _funcionesApi.getHorarioId(idAlum, fechaInicio, fechaFin);
            if (data.Count == 0)
            {
                return NotFound(new ApiResponse<List<HorarioResponse>> { Success = false, Message = "No se encontro horario", Data = [] });
            }
            var apiResult = new ApiResponse<List<HorarioResponse>> { Success = true, Message = "Se encontro horario", Data = data };
            return Ok(apiResult);
        }

        [HttpGet("CursosxId/{idAlum}")]
        public async Task<ActionResult> GetCursos(int idAlum)
        {
            var data = await _funcionesApi.getCursos(idAlum);
            if (data.Count == 0)
            {
                return NotFound(new ApiResponse<List<CursoDTO>> { Success = false, Message = "No se encontro cursos", Data = [] });
            }
            var apiResult = new ApiResponse<List<CursoDTO>> { Success = true, Message = "Se encontro cursos", Data = data };
            return Ok(apiResult);
        }


        [HttpGet("CursosColegioxId/{idAlum}/{anio}/{codPeriodo}")]
        public async Task<ActionResult> GetCursosColegio(int idAlum, int anio, string codPeriodo)
        {
            var data = await _funcionesApi.getCursosColegio(idAlum, anio, codPeriodo);
            if (data.Count == 0)
            {
                return NotFound(new ApiResponse<List<ReporteMatriculaColegioDTO>> { Success = false, Message = "No se encontro cursos", Data = [] });
            }
            var apiResult = new ApiResponse<List<ReporteMatriculaColegioDTO>> { Success = true, Message = "Se encontro cursos", Data = data };
            return Ok(apiResult);
        }

        [HttpGet("Asistencias/{idAlum}/{codCurso}")]
        public async Task<ActionResult> GetAsistenciaAlumno(int idAlum, string codCurso)
        {
            var data = await _funcionesApi.getAsistenciasAlumno(idAlum, codCurso);
            if (data.Count == 0)
            {
                return NotFound(new ApiResponse<List<AlumnoAsistenciaDTO>> { Success = false, Message = "No se encontro asistencias", Data = [] });
            }
            var apiResult = new ApiResponse<List<AlumnoAsistenciaDTO>> { Success = true, Message = "Se encontro asistencias", Data = data };
            return Ok(apiResult);
        }

        [HttpGet("HorariosxAula/{idAula}")]
        public async Task<ActionResult> GetAsistenciaAlumno(int idAula)
        {
            var data = await _funcionesApi.getHorariosxAula(idAula);
            if (data.Count == 0)
            {
                return NotFound(new ApiResponse<List<HorarioxAulaDTO>> { Success = false, Message = "No se encontro horarios", Data = [] });
            }
            var apiResult = new ApiResponse<List<HorarioxAulaDTO>> { Success = true, Message = "Se encontro horarios", Data = data };
            return Ok(apiResult);
        }

        [HttpGet("HorariosCursosxAlumno/{idAlumno}")]
        public async Task<ActionResult> GetHorariosCursoxAlumno(int idAlumno)
        {
            var data = await _funcionesApi.getHorariosCursoxAlumno(idAlumno);
            if (data.Count == 0)
            {
                return NotFound(new ApiResponse<List<HorarioCursoxAlumnnoDTO>> { Success = false, Message = "No se encontro horarios", Data = [] });
            }
            var apiResult = new ApiResponse<List<HorarioCursoxAlumnnoDTO>> { Success = true, Message = "Se encontro horarios", Data = data };
            return Ok(apiResult);
        }

        [HttpGet("HorariosCursosxDocente/{idDocente}")]
        public async Task<ActionResult> GetHorariosCursoxDocente(int idDocente)
        {
            var data = await _funcionesApi.getHorarioCursoxDocente(idDocente);
            if (data.Count == 0)
            {
                return NotFound(new ApiResponse<List<HorarioCursoxDocenteDTO>> { Success = false, Message = "No se encontro horarios", Data = [] });
            }
            var apiResult = new ApiResponse<List<HorarioCursoxDocenteDTO>> { Success = true, Message = "Se encontro horarios", Data = data };
            return Ok(apiResult);
        }

        [HttpGet("NotasxBimestre/{idAlum}/{anio}/{codCurso}/{codSubperiodo}")]
        public async Task<ActionResult> GetNotasxBimestre(int idAlum, int anio, string codCurso, string codSubperiodo)
        {
            var data = await _funcionesApi.getNotasxBimestre(idAlum, anio, codCurso, codSubperiodo);
            if (data.Count == 0)
            {
                return NotFound(new ApiResponse<List<NotasxBimestreDTO>> { Success = false, Message = "No se encontro notas", Data = [] });
            }
            var apiResult = new ApiResponse<List<NotasxBimestreDTO>> { Success = true, Message = "Se encontro notas", Data = data };
            return Ok(apiResult);
        }

        [HttpGet("alumno/pagos-pendientes/{id}/{anio}")]
        public async Task<IActionResult> GetPagosPorAlumno(int id, int anio)
        {
            var pagos = await _funcionesApi.getPagosPorAlumno(id, anio);
            
            if (pagos == null || pagos.Count == 0)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "No se encontraron pagos para este alumno." });
            }
            
            return Ok(new ApiResponse<List<PagoDTO>> { Data = pagos, Message = "Pagos encontrados" });
        }

        [HttpGet("resumen-pagos-alumno/{id}/{anio}")]
        public async Task<IActionResult> GetResumenPagosPorAlumno(int id, int anio)
        {
            var resumen = await _funcionesApi.GetResumenPagosPorAlumno(id, anio);

            if (resumen == null || !resumen.Any())
                return NotFound(new ApiResponse<object> { Success = false, Message = "No se encontró resumen de pagos para este alumno." });

            return Ok(new ApiResponse<List<ResumenPagosDTO?>> { Data = resumen, Message = "Resumen de pagos encontrado" });
        }

        [HttpGet("CalendarioAcademico/{anio}")]
        public async Task<IActionResult> GetCalendarioAcademico(int anio)
        {
            var calendario = await _funcionesApi.GetCalendarioAcademico(anio);

            if (calendario == null || calendario.Count == 0)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = $"No se encontraron actividades para el año {anio}." });
            }

            return Ok(new ApiResponse<List<CalendarioAcademicoDTO>> { Data = calendario, Message = "Calendario académico encontrado" });
        }

        [HttpGet("DocumentosConCategoria")]
        public async Task<ActionResult> GetDocumentosConCategoria()
        {
            var data = await _funcionesApi.GetDocumentosConCategoria();

            if (data == null || !data.Any())
            {
                return NotFound(new ApiResponse<List<CategoriaDocumentoDTO>> { Success = false, Message = "No se encontraron documentos", Data = [] });
            }

            var apiResult = new ApiResponse<List<CategoriaDocumentoDTO>> { Success = true, Message = "Se encontraron documentos", Data = data };
            return Ok(apiResult);
        }

        [HttpGet("ObtenerEventos")]
        public async Task<IActionResult> ObtenerEventos()
        {
            var eventos = await _funcionesApi.GetEventos();

            if (eventos == null || eventos.Count == 0)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "No se encontraron eventos." });
            }

            return Ok(new ApiResponse<List<EventoDTO>> { Data = eventos, Message = "Eventos encontrados" });
        }
    
        [HttpGet("ObtenerObligacionesPagadas/{idAlumno}")]
        public async Task<IActionResult> ObtenerObligacionesPagadas(int idAlumno)
        {
            var obligaciones = await _funcionesApi.GetObligacionesPagadas(idAlumno);

            if (obligaciones == null || obligaciones.Count == 0)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = $"No se encontraron obligaciones pagadas para el alumno con ID {idAlumno}." });
            }

            return Ok(new ApiResponse<List<ObligacionPorPeriodoDTO>> { Data = obligaciones, Message = "Obligaciones encontradas" });
        }

        [HttpPost("registrar-imagen-pago")]
        public async Task<IActionResult> RegistrarImagenPago(ImagenPagoDto imagenPagoDto)
        {
            var status = await _funcionesApi.setImagenPago(imagenPagoDto);
            var apiResult = new ApiResponse<object> { Success = status, Message = "Se registro imagen" };
            return Ok(apiResult);
        }

        [HttpPost("listar-pago-sede")]
        public async Task<IActionResult> ListarPagosPorSede(SedePaginadoDTO sedePaginadoDto)
        {
            var pagos = await _funcionesApi.getPagosPorSede(sedePaginadoDto);

            if (pagos == null || pagos.Count == 0)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "No se encontraron pagos para la sede ingresada." });
            }

            return Ok(new ApiResponse<List<PagoDTO>> { Data = pagos, Message = "Pagos encontrados" });
        }

        [HttpPost("listar-alumno-sede")]
        public async Task<IActionResult> ListarAlumnosPorSede(SedePaginadoDTO listaAlumno)
        {
            var alumnos = await _funcionesApi.getAlumnoPorSede(listaAlumno);

            if (alumnos == null || alumnos.Count == 0)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "No se encontraron alumnos para la sede ingresada." });
            }

            return Ok(new ApiResponse<List<AlumnoDTO>> { Data = alumnos, Message = "Alumnos encontrados" });
        }

        [HttpPost("filtrar-alumno-sede")]
        public async Task<IActionResult> ListarAlumnosPorSede(FiltroAlumnoDTO filtroAlumno)
        {
            var alumnos = await _funcionesApi.filtrarAlumno(filtroAlumno);

            if (alumnos == null || alumnos.Count == 0)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "No se encontraron alumnos." });
            }

            return Ok(new ApiResponse<List<AlumnoDTO>> { Data = alumnos, Message = "Alumnos encontrados" });
        }

        [HttpPost("registrar-usuario-alumno")]
        public async Task<IActionResult> RegistrarUsuarioAlumno(AlumnoRegistrarDTO alumnoRegistrarDto)
        {
            var estado = await _funcionesApi.registrarUsuarioAlumno(alumnoRegistrarDto);
            var apiResult = new ApiResponse<object> { Success = estado, Message = "Se registro usuario" };
            return Ok(apiResult);
        }

        [HttpPut("actualizar-usuario-alumno")]
        public async Task<IActionResult> ActualizarUsuarioAlumno(AlumnoRegistrarDTO alumnoRegistrarDto)
        {
            var estado = await _funcionesApi.actualizarUsuarioAlumno(alumnoRegistrarDto);
            var apiResult = new ApiResponse<object> { Success = estado, Message = "Se actualizo usuario" };
            return Ok(apiResult);
        }

        [HttpDelete("eliminar-usuario-alumno")]
        public async Task<IActionResult> EliminarUsuarioAlumno(string numeroDocumento)
        {
            var estado = await _funcionesApi.eliminarUsuarioAlumno(numeroDocumento);
            var apiResult = new ApiResponse<object> { Success = estado, Message = "Se elimino usuario" };
            return Ok(apiResult);
        }

        [HttpPost("agregar-documento")]
        public async Task<IActionResult> AgregarDocumento(DocumentoAddDTO documentoAddDto)
        {
            var status = await _funcionesApi.AddDocument(documentoAddDto);
            var apiResult = new ApiResponse<object> { Success = status, Message = "Documento agregado correctamente" };
            return Ok(apiResult);
        }

        [HttpGet("listar-grados/{tipoInstitucion}")]
        public async Task<IActionResult> ListarGrados(string tipoInstitucion)
        {
            var grados = await _funcionesApi.GetGrados(tipoInstitucion);

            if (grados == null || grados.Count == 0)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "No se encontraron grados registrados." });
            }

            return Ok(new ApiResponse<List<GradoDTO>> { Data = grados, Message = "Grados encontrados" });
        }

        [HttpPost("listar-cursos")]
        public async Task<IActionResult> ListarCursos(SedePaginadoDTO listaCurso)
        {
            var cursos = await _funcionesApi.ListarCursosPorSede(listaCurso);

            if (cursos == null || !cursos.Any())
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "No se encontraron cursos disponibles." });
            }

            return Ok(new ApiResponse<List<CursoListarDTO>> { Data = cursos, Message = "Cursos encontrados" });
        }

        [HttpPost("filtrar-cursos")]
        public async Task<IActionResult> ListarCursosPorSede(FiltroCursoDTO filtroCurso)
        {
            var cursos = await _funcionesApi.FiltrarCurso(filtroCurso);

            if (cursos == null || cursos.Count == 0)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "No se encontraron cursos." });
            }

            return Ok(new ApiResponse<List<CursoListarDTO>> { Data = cursos, Message = "Cursos encontrados" });
        }

        [HttpPost("registrar-curso")]
        public async Task<IActionResult> RegistrarCurso(CursoRegistrarDTO cursoRegistrarDto)
        {
            var estado = await _funcionesApi.RegistrarCurso(cursoRegistrarDto);
            var apiResult = new ApiResponse<object> { Success = estado, Message = "Se registró el curso correctamente" };
            return Ok(apiResult);
        }

        [HttpPut("actualizar-curso")]
        public async Task<IActionResult> ActualizarCurso(CursoActualizarDTO cursoActualizarDto)
        {
            var estado = await _funcionesApi.ActualizarCurso(cursoActualizarDto);
            var apiResult = new ApiResponse<object> { Success = estado, Message = "Se actualizó el curso correctamente" };
            return Ok(apiResult);
        }

        [HttpDelete("eliminar-curso")]
        public async Task<IActionResult> EliminarCurso(int idCurso)
        {
            var estado = await _funcionesApi.EliminarCurso(idCurso);
            var apiResult = new ApiResponse<object> { Success = estado, Message = "Se eliminó el curso correctamente" };
            return Ok(apiResult);
        }

        [HttpGet("cursos-alumno/{idAlumno}")]
        public async Task<IActionResult> GetCursosAlumno(int idAlumno)
        {
            var cursos = await _funcionesApi.getCursosAlumno(idAlumno);

            if (cursos == null || !cursos.Any())
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "No se encontraron cursos para el alumno." });
            }

            return Ok(new ApiResponse<List<ReporteMatriculaColegioDTO>> { Data = cursos, Message = "Cursos del alumno encontrados" });
        }
    }
}