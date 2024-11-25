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

        public PortalController(IFuncionesApi funcionesApi) {
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
                var apiResult = new ApiResponse<List<AlumnoDTO>>{ Success = true, Message = "Se encontro alumno", Data = data };
                if(data.Count == 0){
                    apiResult = new ApiResponse<List<AlumnoDTO>>{ Success = false, Message = "No se encontro alumno", Data = [] };
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
                var apiResult = new ApiResponse<List<HorarioResponse>>{ Success = true, Message = "Se encontro horario", Data = data };
                if(data.Count == 0){
                    apiResult = new ApiResponse<List<HorarioResponse>>{ Success = false, Message = "No se encontro horario", Data = [] };
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
                var apiResult = new ApiResponse<List<CursoDTO>>{ Success = true, Message = "Se encontro cursos", Data = data };
                if(data.Count == 0){
                    apiResult = new ApiResponse<List<CursoDTO>>{ Success = false, Message = "No se encontro cursos", Data = [] };
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