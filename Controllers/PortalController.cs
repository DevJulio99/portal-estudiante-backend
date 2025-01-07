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

        [HttpGet("AlumnosxId/{idAlum}")]
        public async Task<ActionResult> GetAlumnosId(int idAlum)
        {
            try
            {
                var data = await _funcionesApi.getAlumnosId(idAlum);
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
    }
}