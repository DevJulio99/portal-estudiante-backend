using System.ComponentModel.DataAnnotations;

namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class ExamenGeneradoDTO
    {
        public required int idExamenGenerado { get; set; }
        public required int idPostulante { get; set; }
        public required int idPregunta { get; set; }
        public required int ordenPregunta { get; set; }
        public required string respuestaSeleccionada { get; set; }
        public required string tiempoRespuesta { get; set; }
        public required Boolean completado { get; set; }
    }
}
