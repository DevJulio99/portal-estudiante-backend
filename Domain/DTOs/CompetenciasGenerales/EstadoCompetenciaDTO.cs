namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class EstadoCompetenciaDTO
    {
        public required int idCompetencia { get; set; }
        public required int idPostulante { get; set; }
        public string? estado { get; set; }
    }
}
