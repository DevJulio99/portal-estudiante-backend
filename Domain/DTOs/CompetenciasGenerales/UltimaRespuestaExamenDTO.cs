namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class UltimaRespuestaExamenDTO
    {
        public required int idPregunta { get; set; }
        public string? respuestaSelecciona { get; set; }
        public string? tiempoRespuesta { get; set; }
    }
}
