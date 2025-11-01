namespace MyPortalStudent.Domain.DTOs.Notas
{
    public class NotasAlumnoDTO
    {
        public int IdNota { get; set; }
        public decimal? Nota { get; set; }
        public decimal? Peso { get; set; }
        public string TipoNota { get; set; } = string.Empty;
        public int IdAlumno { get; set; }
    }
}