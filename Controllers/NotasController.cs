using Microsoft.AspNetCore.Mvc;
using MyPortalStudent.Domain;
using MyPortalStudent.Domain.DTOs;
using MyPortalStudent.Domain.DTOs.Notas;
using MyPortalStudent.Domain.IServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPortalStudent.Controllers
{
    [Route("api/v1/notas")]
    [ApiController]
    public class NotasController : ControllerBase
    {
        private readonly INotasService _notasService;

        public NotasController(INotasService notasService)
        {
            _notasService = notasService;
        }

        [HttpPost("periodo-sede")]
        public async Task<ActionResult> GetPeriodoPorSede([FromBody] PeriodoRequestDTO request)
        {
            var periodos = await _notasService.GetPeriodoPorSede(request);

            if (periodos == null || !periodos.Any())
            {
                return NotFound(new ApiResponse<List<PeriodoSedeDTO>>
                {
                    Success = false,
                    Message = "No se encontraron periodos para la sede especificada.",
                    Data = []
                });
            }

            return Ok(new ApiResponse<List<PeriodoSedeDTO>> { Success = true, Message = "Periodos encontrados.", Data = periodos });
        }

        [HttpPost("grado-sede")]
        public async Task<ActionResult> GetGradoPorSede([FromBody] GradoRequestDTO request)
        {
            var grados = await _notasService.GetGradoPorSede(request);

            if (grados == null || !grados.Any())
            {
                return NotFound(new ApiResponse<List<GradoSedeDTO>>
                {
                    Success = false,
                    Message = "No se encontraron grados para la sede especificada.",
                    Data = []
                });
            }

            return Ok(new ApiResponse<List<GradoSedeDTO>> { Success = true, Message = "Grados encontrados.", Data = grados });
        }

        [HttpPost("subperiodos")]
        public async Task<ActionResult> GetSubperiodosPorPeriodo([FromBody] SubperiodoRequestDTO request)
        {
            var subperiodos = await _notasService.GetSubperiodosPorPeriodo(request);

            if (subperiodos == null || !subperiodos.Any())
            {
                return NotFound(new ApiResponse<List<SubperiodoDTO>>
                {
                    Success = false,
                    Message = "No se encontraron subperiodos para el periodo especificado.",
                    Data = []
                });
            }

            return Ok(new ApiResponse<List<SubperiodoDTO>> { Success = true, Message = "Subperiodos encontrados.", Data = subperiodos });
        }

        [HttpPost("secciones-por-grado")]
        public async Task<ActionResult> GetSeccionesPorGrado([FromBody] SeccionesRequestDTO request)
        {
            var secciones = await _notasService.GetSeccionesPorGrado(request);

            if (secciones == null || !secciones.Any())
            {
                return NotFound(new ApiResponse<List<SeccionGradoDTO>>
                {
                    Success = false,
                    Message = "No se encontraron secciones para los filtros especificados.",
                    Data = []
                });
            }

            return Ok(new ApiResponse<List<SeccionGradoDTO>>
            {
                Success = true, Message = "Secciones encontradas.", Data = secciones
            });
        }

        [HttpPost("cursos-por-grado")]
        public async Task<ActionResult> GetCursosPorGrado([FromBody] CursosPorGradoSedeRequestDTO request)
        {
            var cursos = await _notasService.GetCursosPorGrado(request);

            if (cursos == null || !cursos.Any())
            {
                return NotFound(new ApiResponse<List<CursoGradoDTO>>
                {
                    Success = false,
                    Message = "No se encontraron cursos para los filtros especificados.",
                    Data = []
                });
            }

            return Ok(new ApiResponse<List<CursoGradoDTO>>
            {
                Success = true,
                Message = "Cursos encontrados.",
                Data = cursos
            });
        }

        [HttpPost("alumnos-por-filtro")]
        public async Task<ActionResult> GetAlumnosPorFiltro([FromBody] AlumnosPorFiltroRequestDTO request)
        {
            var alumnos = await _notasService.GetAlumnosPorFiltro(request);

            if (alumnos == null || !alumnos.Any())
            {
                return NotFound(new ApiResponse<List<AlumnoFiltroDTO>>
                {
                    Success = false,
                    Message = "No se encontraron alumnos para los filtros especificados.",
                    Data = []
                });
            }

            return Ok(new ApiResponse<List<AlumnoFiltroDTO>>
            {
                Success = true,
                Message = "Alumnos encontrados.",
                Data = alumnos
            });
        }

        [HttpPost("notas-alumno")]
        public async Task<ActionResult> GetNotasAlumno([FromBody] NotasAlumnoRequestDTO request)
        {
            var notas = await _notasService.GetNotasAlumno(request);

            if (notas == null || !notas.Any())
            {
                return NotFound(new ApiResponse<List<NotasAlumnoDTO>>
                {
                    Success = false,
                    Message = "No se encontraron notas para los filtros especificados.",
                    Data = []
                });
            }

            return Ok(new ApiResponse<List<NotasAlumnoDTO>>
            {
                Success = true,
                Message = "Notas encontradas.",
                Data = notas
            });
        }

        [HttpPost("registrar")]
        public async Task<ActionResult> RegistrarNotasAlumno([FromBody] RegistrarNotaDto request)
        {
            var resultado = await _notasService.RegistrarNotasAlumno(request);

            return Ok(new ApiResponse<BaseResponseDTO> { Success = true, Message = resultado.Message, Data = resultado });
        }
    }
}