using System.ComponentModel.DataAnnotations;

namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class GenerarExamenDTO
    {
        [Required(ErrorMessage = "Por favor, ingresa el Id del postulante.")]
        public required int idPostulante { get; set; }
        [Required(ErrorMessage = "Por favor, ingresa el numero de preguntas.")]
        public required int numeroPreguntas { get; set; }
        [Required(ErrorMessage = "Por favor, ingresa el Id de la competencia.")]
        public required int idCompetencia { get; set; }
    }
}
