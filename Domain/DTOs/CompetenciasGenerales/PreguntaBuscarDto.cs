using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class PreguntaBuscarDto
    {
        
        [Range(1, int.MaxValue, ErrorMessage = "El valor del TipoEvaluacion debe ser un número entero positivo.")]
        public int? TipoEvaluacion { get; set; }

        [DefaultValue(1)]
        [Required(ErrorMessage = "El número de página es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser un valor positivo.")]
        public int PageNumber { get; set; }

        [DefaultValue(10)]
        [Required(ErrorMessage = "El tamaño de página es obligatorio.")]
        [Range(1, 1000, ErrorMessage = "El tamaño de página debe estar entre 1 y 1000.")]
        public int PageSize { get; set; }
    }
}
