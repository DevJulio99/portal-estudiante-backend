using Microsoft.AspNetCore.Mvc;
using MyPortalStudent.Domain.IServices;
using MyPortalStudent.Domain.Dtos.AulaVirtual;

namespace MyPortalStudent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AulaVirtualController : ControllerBase
    {
        private readonly IAulaVirtualService _aulaVirtualService;

        public AulaVirtualController(IAulaVirtualService aulaVirtualService)
        {
            _aulaVirtualService = aulaVirtualService;
        }

        [HttpPost("silabo")]
        public async Task<IActionResult> GetSilaboPorCurso([FromBody] SilaboRequestDto request)
        {
            var resultado = await _aulaVirtualService.GetSilaboPorCursoAsync(request);
            if (resultado == null || !resultado.Success)
                return NotFound(resultado);

            return Ok(resultado);
        }

        [HttpPost("materiales")]
        public async Task<IActionResult> GetMateriales([FromBody] MaterialesRequestDto request)
        {
            var resultado = await _aulaVirtualService.GetMateriales(request);
            if (resultado == null || !resultado.Success)
                return NotFound(resultado);

            return Ok(resultado);
        }

        [HttpPost("registrar-material")]
        public async Task<IActionResult> RegistrarMaterial([FromBody] RegistrarMaterialRequestDto request)
        {
            var resultado = await _aulaVirtualService.RegistrarMaterial(request);
            if (resultado == null || !resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}