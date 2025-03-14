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
            try
            {
                var data = await _funcionesApi.getAlumnos();
                var apiResult = new { Success = true, Data = data };
                return Ok(apiResult);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return Unauthorized(ex);
                    case ArgumentException _:
                        return BadRequest(ex);
                    default:
                        return StatusCode(500, ex);
                }
            }
        }

        [HttpGet("AlumnosxId/{numDocUsuario}")]
        public async Task<ActionResult> GetAlumnosId(string? numDocUsuario)
        {
            try
            {
                var data = await _funcionesApi.getAlumnosId(numDocUsuario);
                var apiResult = new ApiResponse<List<PerfilDTO>> { Success = true, Message = "Se encontro alumno", Data = data };
                if (data.Count == 0)
                {
                    apiResult = new ApiResponse<List<PerfilDTO>> { Success = false, Message = "No se encontro alumno", Data = [] };
                    return NotFound(apiResult);
                }
                return Ok(apiResult);
            }
            catch (Exception ex)
            {
                var errResponse = new { Success = false, Message = ex.Message, Data = "" };
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return Unauthorized(errResponse);
                    case ArgumentException _:
                        return BadRequest(errResponse);
                    default:
                        return StatusCode(500, errResponse);
                }
            }
        }

        [HttpGet("HorarioxId/{idAlum}/{fechaInicio}/{fechaFin}")]
        public async Task<ActionResult> GetHorarioId(int idAlum, string fechaInicio, string fechaFin)
        {
            try
            {
                var data = await _funcionesApi.getHorarioId(idAlum, fechaInicio, fechaFin);
                var apiResult = new ApiResponse<List<HorarioResponse>> { Success = true, Message = "Se encontro horario", Data = data };
                if (data.Count == 0)
                {
                    apiResult = new ApiResponse<List<HorarioResponse>> { Success = false, Message = "No se encontro horario", Data = [] };
                    return NotFound(apiResult);
                }
                return Ok(apiResult);
            }
            catch (Exception ex)
            {
                var errResponse = new { Success = false, Message = ex.Message, Data = "" };
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return Unauthorized(errResponse);
                    case ArgumentException _:
                        return BadRequest(errResponse);
                    default:
                        return StatusCode(500, errResponse);
                }
            }
        }

        [HttpGet("CursosxId/{idAlum}")]
        public async Task<ActionResult> GetCursos(int idAlum)
        {
            try
            {
                var data = await _funcionesApi.getCursos(idAlum);
                var apiResult = new ApiResponse<List<CursoDTO>> { Success = true, Message = "Se encontro cursos", Data = data };
                if (data.Count == 0)
                {
                    apiResult = new ApiResponse<List<CursoDTO>> { Success = false, Message = "No se encontro cursos", Data = [] };
                    return NotFound(apiResult);
                }
                return Ok(apiResult);
            }
            catch (Exception ex)
            {
                var errResponse = new { Success = false, Message = ex.Message, Data = "" };
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return Unauthorized(errResponse);
                    case ArgumentException _:
                        return BadRequest(errResponse);
                    default:
                        return StatusCode(500, errResponse);
                }
            }
        }


        [HttpGet("CursosColegioxId/{idAlum}/{anio}")]
        public async Task<ActionResult> GetCursosColegio(int idAlum, int anio)
        {
            try
            {
                var data = await _funcionesApi.getCursosColegio(idAlum, anio);
                var apiResult = new ApiResponse<List<ReporteMatriculaColegioDTO>> { Success = true, Message = "Se encontro cursos", Data = data };
                if (data.Count == 0)
                {
                    apiResult = new ApiResponse<List<ReporteMatriculaColegioDTO>> { Success = false, Message = "No se encontro cursos", Data = [] };
                    return NotFound(apiResult);
                }
                return Ok(apiResult);
            }
            catch (Exception ex)
            {
                var errResponse = new { Success = false, Message = ex.Message, Data = "" };
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return Unauthorized(errResponse);
                    case ArgumentException _:
                        return BadRequest(errResponse);
                    default:
                        return StatusCode(500, errResponse);
                }
            }
        }

        [HttpGet("Asistencias/{idAlum}/{bimester}/{codCurso}/{anio}")]
        public async Task<ActionResult> GetAsistenciaAlumno(int idAlum, string bimester, string codCurso, int anio)
        {
            try
            {
                var data = await _funcionesApi.getAsistenciasAlumno(idAlum,bimester, codCurso, anio);
                var apiResult = new ApiResponse<List<AlumnoAsistenciaDTO>> { Success = true, Message = "Se encontro asistencias", Data = data };
                if (data.Count == 0)
                {
                    apiResult = new ApiResponse<List<AlumnoAsistenciaDTO>> { Success = false, Message = "No se encontro asistencias", Data = [] };
                    return NotFound(apiResult);
                }
                return Ok(apiResult);
            }
            catch (Exception ex)
            {
                var errResponse = new { Success = false, Message = ex.Message, Data = "" };
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return Unauthorized(errResponse);
                    case ArgumentException _:
                        return BadRequest(errResponse);
                    default:
                        return StatusCode(500, errResponse);
                }
            }
        }

        [HttpGet("HorariosxAula/{idAula}")]
        public async Task<ActionResult> GetAsistenciaAlumno(int idAula)
        {
            try
            {
                var data = await _funcionesApi.getHorariosxAula(idAula);
                var apiResult = new ApiResponse<List<HorarioxAulaDTO>> { Success = true, Message = "Se encontro horarios", Data = data };
                if (data.Count == 0)
                {
                    apiResult = new ApiResponse<List<HorarioxAulaDTO>> { Success = false, Message = "No se encontro horarios", Data = [] };
                    return NotFound(apiResult);
                }
                return Ok(apiResult);
            }
            catch (Exception ex)
            {
                var errResponse = new { Success = false, Message = ex.Message, Data = "" };
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return Unauthorized(errResponse);
                    case ArgumentException _:
                        return BadRequest(errResponse);
                    default:
                        return StatusCode(500, errResponse);
                }
            }
        }

        [HttpGet("HorariosCursosxAlumno/{idAlumno}")]
        public async Task<ActionResult> GetHorariosCursoxAlumno(int idAlumno)
        {
            try
            {
                var data = await _funcionesApi.getHorariosCursoxAlumno(idAlumno);
                var apiResult = new ApiResponse<List<HorarioCursoxAlumnnoDTO>> { Success = true, Message = "Se encontro horarios", Data = data };
                if (data.Count == 0)
                {
                    apiResult = new ApiResponse<List<HorarioCursoxAlumnnoDTO>> { Success = false, Message = "No se encontro horarios", Data = [] };
                    return NotFound(apiResult);
                }
                return Ok(apiResult);
            }
            catch (Exception ex)
            {
                var errResponse = new { Success = false, Message = ex.Message, Data = "" };
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return Unauthorized(errResponse);
                    case ArgumentException _:
                        return BadRequest(errResponse);
                    default:
                        return StatusCode(500, errResponse);
                }
            }
        }

        [HttpGet("HorariosCursosxDocente/{idDocente}")]
        public async Task<ActionResult> GetHorariosCursoxDocente(int idDocente)
        {
            try
            {
                var data = await _funcionesApi.getHorarioCursoxDocente(idDocente);
                var apiResult = new ApiResponse<List<HorarioCursoxDocenteDTO>> { Success = true, Message = "Se encontro horarios", Data = data };
                if (data.Count == 0)
                {
                    apiResult = new ApiResponse<List<HorarioCursoxDocenteDTO>> { Success = false, Message = "No se encontro horarios", Data = [] };
                    return NotFound(apiResult);
                }
                return Ok(apiResult);
            }
            catch (Exception ex)
            {
                var errResponse = new { Success = false, Message = ex.Message, Data = "" };
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return Unauthorized(errResponse);
                    case ArgumentException _:
                        return BadRequest(errResponse);
                    default:
                        return StatusCode(500, errResponse);
                }
            }
        }

        [HttpGet("NotasxBimestre/{idAlum}/{tipoPeriodo}/{anio}")]
        public async Task<ActionResult> GetNotasxBimestre(int idAlum, string tipoPeriodo, int anio)
        {
            try
            {
                var data = await _funcionesApi.getNotasxBimestre(idAlum, tipoPeriodo, anio);
                var apiResult = new ApiResponse<List<NotasxBimestreDTO>> { Success = true, Message = "Se encontro notas", Data = data };
                if (data.Count == 0)
                {
                    apiResult = new ApiResponse<List<NotasxBimestreDTO>> { Success = false, Message = "No se encontro notas", Data = [] };
                    return NotFound(apiResult);
                }
                return Ok(apiResult);
            }
            catch (Exception ex)
            {
                var errResponse = new { Success = false, Message = ex.Message, Data = "" };
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return Unauthorized(errResponse);
                    case ArgumentException _:
                        return BadRequest(errResponse);
                    default:
                        return StatusCode(500, errResponse);
                }
            }
        }

        [HttpGet("alumno/{id}/{anio}")]
        public async Task<IActionResult> GetPagosPorAlumno(int id, int anio)
        {
            try
            {
                var pagos = await _funcionesApi.getPagosPorAlumno(id, anio);
                
                if (pagos == null || pagos.Count == 0)
                {
                    return NotFound("No se encontraron pagos para este alumno.");
                }
                
                var apiResult = new { Success = true, Data = pagos };
                return Ok(apiResult);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("CalendarioAcademico/{anio}")]
        public async Task<IActionResult> GetCalendarioAcademico(int anio)
        {
            try
            {
                var calendario = await _funcionesApi.GetCalendarioAcademico(anio);

                if (calendario == null || calendario.Count == 0)
                {
                    return NotFound($"No se encontraron actividades para el año {anio}.");
                }

                var apiResult = new { Success = true, Data = calendario };
                return Ok(apiResult);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("DocumentosConCategoria")]
        public async Task<ActionResult> GetDocumentosConCategoria()
        {
            try
            {
                var data = await _funcionesApi.GetDocumentosConCategoria();
                var apiResult = new ApiResponse<List<CategoriaDocumentoDTO>> { Success = true, Message = "Se encontraron documentos", Data = data };

                if (data == null || !data.Any())
                {
                    apiResult = new ApiResponse<List<CategoriaDocumentoDTO>> { Success = false, Message = "No se encontraron documentos", Data = new List<CategoriaDocumentoDTO>() };
                    return NotFound(apiResult);
                }

                return Ok(apiResult);
            }
            catch (Exception ex)
            {
                var errResponse = new { Success = false, Message = ex.Message, Data = "" };
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return Unauthorized(errResponse);
                    case ArgumentException _:
                        return BadRequest(errResponse);
                    default:
                        return StatusCode(500, errResponse);
                }
            }
        }

        [HttpGet("ObtenerEventos")]
        public async Task<IActionResult> ObtenerEventos()
        {
            try
            {
                var eventos = await _funcionesApi.GetEventos();

                if (eventos == null || eventos.Count == 0)
                {
                    return NotFound($"No se encontraron eventos.");
                }

                var apiResult = new { Success = true, eventos.Count, Data = eventos };
                return Ok(apiResult);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    
        [HttpGet("ObtenerObligacionesPagadas/{idAlumno}")]
        public async Task<IActionResult> ObtenerObligacionesPagadas(int idAlumno)
        {
            try
            {
                var obligaciones = await _funcionesApi.GetObligacionesPagadas(idAlumno);

                if (obligaciones == null || obligaciones.Count == 0)
                {
                    return NotFound($"No se encontraron obligaciones pagadas para el alumno con ID {idAlumno}.");
                }

                var apiResult = new { Success = true, obligaciones.Count, Data = obligaciones };
                return Ok(apiResult);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("registrar-imagen-pago")]
        public async Task<IActionResult> RegistrarImagenPago(ImagenPagoDto imagenPagoDto)
        {
            try
            {
                var status = await _funcionesApi.setImagenPago(imagenPagoDto);
                var apiResult = new { Success = status,Message = "Se registro imagen", Data = "" };
                return Ok(apiResult);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("listar-pago-sede/{codigoSede}")]
        public async Task<IActionResult> ListarPagosPorSede(string codigoSede)
        {
            try
            {
                var pagos = await _funcionesApi.getPagosPorSede(codigoSede);

                if (pagos == null || pagos.Count == 0)
                {
                    return NotFound($"No se encontraron pagos para la sede ingresada.");
                }

                var apiResult = new { Success = true, pagos.Count, Data = pagos };
                return Ok(apiResult);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("listar-alumno-sede/{codigoSede}")]
        public async Task<IActionResult> ListarAlumnosPorSede(string codigoSede)
        {
            try
            {
                var alumnos = await _funcionesApi.getAlumnoPorSede(codigoSede);

                if (alumnos == null || alumnos.Count == 0)
                {
                    return NotFound($"No se encontraron alumnos para la sede ingresada.");
                }

                var apiResult = new { Success = true, alumnos.Count, Data = alumnos };
                return Ok(apiResult);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                var error = new{ Success = false, Message = ex.Message, Data = "" };
                return StatusCode(500, error);
            }
        }

        [HttpPost("registrar-usuario-alumno")]
        public async Task<IActionResult> RegistrarUsuarioAlumno(AlumnoRegistrarDTO alumnoRegistrarDto)
        {
            try
            {
                var estado = await _funcionesApi.registrarUsuarioAlumno(alumnoRegistrarDto);

                var apiResult = new { Success = estado, Message = "Se registro usuario", Data = "" };
                return Ok(apiResult);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex);
            }
            catch (ArgumentException ex)
            {
                var error = new{ Success = false, Message = ex.Message, Data = "" };
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                var error = new{ Success = false, Message = ex.Message, Data = "" };
                return StatusCode(500, error);
            }
        }

        [HttpPut("actualizar-usuario-alumno")]
        public async Task<IActionResult> ActualizarUsuarioAlumno(AlumnoRegistrarDTO alumnoRegistrarDto)
        {
            try
            {
                var estado = await _funcionesApi.actualizarUsuarioAlumno(alumnoRegistrarDto);

                var apiResult = new { Success = true, Message = "Se actualizo usuario", Data = "" };
                return Ok(apiResult);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex);
            }
            catch (ArgumentException ex)
            {
                var error = new{ Success = false, Message = ex.Message, Data = "" };
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                var error = new{ Success = false, Message = ex.Message, Data = "" };
                return StatusCode(500, error);
            }
        }

        [HttpDelete("eliminar-usuario-alumno")]
        public async Task<IActionResult> EliminarUsuarioAlumno(string numeroDocumento)
        {
            try
            {
                var estado = await _funcionesApi.eliminarUsuarioAlumno(numeroDocumento);

                var apiResult = new { Success = true, Message = "Se elimino usuario", Data = "" };
                return Ok(apiResult);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex);
            }
            catch (ArgumentException ex)
            {
                var error = new{ Success = false, Message = ex.Message, Data = "" };
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                var error = new{ Success = false, Message = ex.Message, Data = "" };
                return StatusCode(500, error);
            }
        }

        [HttpPost("agregar-documento")]
        public async Task<IActionResult> AgregarDocumento(DocumentoAddDTO documentoAddDto)
        {
            try
            {
                var status = await _funcionesApi.AddDocument(documentoAddDto);
                var apiResult = new { Success = status, Message = "Documento agregado correctamente", Data = "" };
                return Ok(apiResult);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("listar-grados")]
        public async Task<IActionResult> ListarGrados()
        {
            try
            {
                var grados = await _funcionesApi.GetGrados();

                if (grados == null || grados.Count == 0)
                {
                    return NotFound("No se encontraron grados registrados.");
                }

                var apiResult = new { Success = true, Count = grados.Count, Data = grados };
                return Ok(apiResult);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                var error = new { Success = false, Message = ex.Message, Data = "" };
                return StatusCode(500, error);
            }
        }
    }
}