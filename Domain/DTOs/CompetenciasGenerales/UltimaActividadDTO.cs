namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class UltimaActividadDTO
    {
        public required int idPostulante { get; set; }
        public required int idCompetencia { get; set; }
        public required string fechaUltimaActividad { get; set; }
        public required string horaUltimaActividad { get; set; }
    }
}