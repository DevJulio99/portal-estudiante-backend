using MyPortalStudent.Domain.DTOs.CompetenciasGenerales;
using System.ComponentModel.DataAnnotations;

namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class ExamenDTO
    {
        public required string nombreCompetencia { get; set; }
        public required ExamenGeneradoDTO examenGenerado { get; set; }
        public required PreguntaDTO2 preguntas { get; set; }
    }
}
