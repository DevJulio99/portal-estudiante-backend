namespace MyPortalStudent.Domain.DTOs
{
    public class ReporteNotaDTO
    {
        public int IdSubperiodo { get; set; }
        public string? DescripcionSubperiodo { get; set; }
        public string? DescripcionCurso { get; set; }
        public decimal PromedioCurso { get; set; }
        public decimal PromedioBimestre { get; set; }
        public decimal PromedioAnual { get; set; }
    }
}