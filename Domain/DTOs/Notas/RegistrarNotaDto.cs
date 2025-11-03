namespace MyPortalStudent.Domain.DTOs.Notas
{
    public class RegistrarNotaDto
    {
        public int IdAlumno { get; set; }
        public int IdCurso { get; set; }
        public int IdPeriodo { get; set; }
        public string TipoNota { get; set; } = string.Empty;
        public decimal Nota { get; set; }
        public decimal Peso { get; set; }
        public int? IdSubperiodo { get; set; }
    }
}