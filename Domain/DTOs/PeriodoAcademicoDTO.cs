namespace MyPortalStudent.Domain.DTOs
{
    public class PeriodoAcademicoDTO
    {
        public int IdPeriodo { get; set; }
        public string CodigoPeriodo { get; set; } = string.Empty;
        public string? DescripcionPeriodo { get; set; }
        public string? TipoPeriodo { get; set; }
        public string? EstadoPeriodo { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? Anio { get; set; }
    }
}