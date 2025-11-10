namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class ListaEstadoCompetenciaDTO : EstadoCompetenciaDTO
    {
        public string tiempoIniciado { get; set; }
        public string tiempoFinalizado { get; set; }
        public int ultimaPregunta { get; set; }
        public int tiempoUltimaPregunta { get; set; }
    }
}
