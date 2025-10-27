using Microsoft.AspNetCore.Mvc;
using MyPortalStudent.Domain;
using MyPortalStudent.Domain.DTOs;
using MyPortalStudent.Domain.IServices;

namespace MyPortalStudent.Controllers
{
    [Route("api/v1/matricula")]
    [ApiController]
    public class MatriculaController : ControllerBase
    {
        private readonly IMatriculaService _matriculaService;

        public MatriculaController(IMatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
        }

        /// <summary>
        /// Realiza una nueva matrícula para un alumno
        /// </summary>
        /// <param name="matriculaDto">Datos de la matrícula a registrar</param>
        /// <returns>Resultado de la operación de matrícula</returns>
        [HttpPost("realizar")]
        public async Task<ActionResult> RealizarMatricula([FromBody] MatriculaRegistrarDTO matriculaDto)
        {
            var resultado = await _matriculaService.RealizarMatricula(matriculaDto);
            
            if (resultado.Success)
            {
                return Ok(new ApiResponse<MatriculaResponseDTO>
                {
                    Success = true,
                    Message = resultado.Message,
                    Data = resultado
                });
            }
            else
            {
                // Alineando con el patrón de PortalController de retornar Ok con Success = false para fallos de lógica de negocio
                return Ok(new ApiResponse<MatriculaResponseDTO>
                {
                    Success = false,
                    Message = resultado.Message,
                    Data = resultado
                });
            }
        }

        /// <summary>
        /// Lista las matrículas por período académico
        /// </summary>
        /// <param name="idPeriodo">ID del período académico</param>
        /// <param name="codigoSede">Código de la sede (opcional)</param>
        /// <returns>Lista de matrículas del período</returns>
        [HttpGet("listar-por-periodo/{idPeriodo}")]
        public async Task<ActionResult> ListarMatriculasPorPeriodo(int idPeriodo, [FromQuery] string? codigoSede = null)
        {
            var matriculas = await _matriculaService.ListarMatriculasPorPeriodo(idPeriodo, codigoSede);
            
            if (matriculas.Count == 0)
            {
                return NotFound(new ApiResponse<List<MatriculaListarDTO>>
                {
                    Success = false,
                    Message = "No se encontraron matrículas para el período especificado",
                    Data = []
                });
            }

            return Ok(new ApiResponse<List<MatriculaListarDTO>>
            {
                Success = true,
                Message = "Matrículas encontradas",
                Data = matriculas
            });
        }

        /// <summary>
        /// Obtiene una matrícula por su ID
        /// </summary>
        /// <param name="idMatricula">ID de la matrícula</param>
        /// <returns>Datos completos de la matrícula</returns>
        [HttpGet("obtener/{idMatricula}")]
        public async Task<ActionResult> ObtenerMatriculaPorId(int idMatricula)
        {
            var matricula = await _matriculaService.ObtenerMatriculaPorId(idMatricula);
            
            if (matricula == null)
            {
                return NotFound(new ApiResponse<MatriculaDTO>
                {
                    Success = false,
                    Message = "No se encontró la matrícula especificada"
                });
            }

            return Ok(new ApiResponse<MatriculaDTO>
            {
                Success = true,
                Message = "Matrícula encontrada",
                Data = matricula
            });
        }

        /// <summary>
        /// Obtiene todas las matrículas de un alumno
        /// </summary>
        /// <param name="idAlumno">ID del alumno</param>
        /// <returns>Lista de matrículas del alumno</returns>
        [HttpGet("obtener-por-alumno/{idAlumno}")]
        public async Task<ActionResult> ObtenerMatriculasPorAlumno(int idAlumno)
        {
            var matriculas = await _matriculaService.ObtenerMatriculasPorAlumno(idAlumno);
            
            if (matriculas.Count == 0)
            {
                return NotFound(new ApiResponse<List<MatriculaDTO>>
                {
                    Success = false,
                    Message = "No se encontraron matrículas para el alumno especificado",
                    Data = []
                });
            }

            return Ok(new ApiResponse<List<MatriculaDTO>>
            {
                Success = true,
                Message = "Matrículas del alumno encontradas",
                Data = matriculas
            });
        }

        /// <summary>
        /// Verifica si un alumno puede matricularse en un período
        /// </summary>
        /// <param name="idAlumno">ID del alumno</param>
        /// <param name="idPeriodo">ID del período académico</param>
        /// <returns>True si puede matricularse, false en caso contrario</returns>
        [HttpGet("verificar-matricula/{idAlumno}/{idPeriodo}")]
        public async Task<ActionResult> VerificarMatriculaAlumno(int idAlumno, int idPeriodo)
        {
            var puedeMatricularse = await _matriculaService.VerificarMatriculaAlumno(idAlumno, idPeriodo);
            
            return Ok(new ApiResponse<bool>
            {
                Success = true, // La llamada a la API fue exitosa, el campo 'Data' indica el resultado de la lógica de negocio
                Message = puedeMatricularse ? "El alumno puede matricularse" : "El alumno ya tiene una matrícula activa para este período",
                Data = puedeMatricularse
            });
        }

        /// <summary>
        /// Actualiza el estado de una matrícula
        /// </summary>
        /// <param name="idMatricula">ID de la matrícula</param>
        /// <param name="nuevoEstado">Nuevo estado de la matrícula</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("actualizar-estado/{idMatricula}")]
        public async Task<ActionResult> ActualizarEstadoMatricula(int idMatricula, [FromBody] string nuevoEstado)
        {
            var resultado = await _matriculaService.ActualizarEstadoMatricula(idMatricula, nuevoEstado);
            
            if (resultado)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Estado de matrícula actualizado correctamente"
                });
            }
            else
            {
                // Alineando con el patrón de PortalController de retornar Ok con Success = false para fallos de lógica de negocio
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No se pudo actualizar el estado de la matrícula"
                });
            }
        }

        /// <summary>
        /// Desactiva una matrícula
        /// </summary>
        /// <param name="idMatricula">ID de la matrícula</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("desactivar/{idMatricula}")]
        public async Task<ActionResult> DesactivarMatricula(int idMatricula)
        {
            var resultado = await _matriculaService.DesactivarMatricula(idMatricula);
            
            if (resultado)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Matrícula desactivada correctamente"
                });
            }
            else
            {
                // Alineando con el patrón de PortalController de retornar Ok con Success = false para fallos de lógica de negocio
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No se pudo desactivar la matrícula"
                });
            }
        }

        /// <summary>
        /// Obtiene todas las matrículas activas de una sede
        /// </summary>
        /// <param name="codigoSede">Código de la sede</param>
        /// <returns>Lista de matrículas activas de la sede</returns>
        [HttpGet("obtener-por-sede/{codigoSede}")]
        public async Task<ActionResult> ObtenerMatriculasActivasPorSede(string codigoSede)
        {
            var matriculas = await _matriculaService.ObtenerMatriculasActivasPorSede(codigoSede);
            
            if (matriculas.Count == 0)
            {
                return NotFound(new ApiResponse<List<MatriculaDTO>>
                {
                    Success = false,
                    Message = "No se encontraron matrículas activas para la sede especificada",
                    Data = []
                });
            }

            return Ok(new ApiResponse<List<MatriculaDTO>>
            {
                Success = true,
                Message = "Matrículas de la sede encontradas",
                Data = matriculas
            });
        }

        /// <summary>
        /// Obtiene los períodos académicos disponibles para matrícula (activos y futuros).
        /// </summary>
        /// <returns>Una lista de períodos académicos.</returns>
        [HttpGet("periodos-disponibles/{codigoSede}")]
        public async Task<ActionResult> ListarPeriodosDisponibles(string codigoSede)
        {
            var periodos = await _matriculaService.ListarPeriodosDisponiblesParaMatricula(codigoSede);

            return Ok(new ApiResponse<List<PeriodoAcademicoDTO>>
            {
                Success = true,
                Message = periodos.Any() ? "Períodos encontrados." : "No se encontraron períodos disponibles para matrícula.",
                Data = periodos
            });
        }

        [HttpPost("cursos-por-grado")]
        public async Task<ActionResult> GetCursosPorGrado([FromBody] CursosPorGradoRequestDTO request)
        {
            var cursos = await _matriculaService.GetCursosPorGrado(request.IdGrado, request.TipoInstitucion);

            if (cursos == null || !cursos.Any())
            {
                return NotFound(new ApiResponse<List<CursoSeccionDTO>>
                {
                    Success = false,
                    Message = "No se encontraron cursos para el grado y tipo de institución especificados.",
                    Data = []
                });
            }

            return Ok(new ApiResponse<List<CursoSeccionDTO>> { Success = true, Message = "Cursos encontrados.", Data = cursos });
        }
    }
}
