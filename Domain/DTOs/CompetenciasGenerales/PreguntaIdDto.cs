using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class PreguntaIdDto
    {
        [DefaultValue(1)]
        [Range(1, int.MaxValue, ErrorMessage = "El valor del id debe ser un número entero positivo.")]
        [Required(ErrorMessage = "El valor de id es requerido")]
        public int id { get; set; }
    }
}
