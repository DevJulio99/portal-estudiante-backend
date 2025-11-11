using System.ComponentModel.DataAnnotations;

namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class RespuestaReq
    {
        [Required(ErrorMessage = "Por favor, ingresa el Id del postulante.")]
        public required int idPostulante { get; set; }
        [Required(ErrorMessage = "Por favor, ingresa el Id de la pregunta.")]
        public required int idPregunta { get; set; }
        [Required(ErrorMessage = "Por favor, ingresa la respuesta seleccionada.")]
        public required string respuestSeleccionada { get; set; }

        public required int idCompetencia { get; set; }
        public required int ultimaPregunta { get; set; }
        public required int tiempoUltimaPregunta { get; set; }
    }
}
