namespace MyPortalStudent.Domain.DTOs.Notas
{
    public class NotasAlumnoRequestDTO
    {
        public int? IdAlumno { get; set; }
        public int? IdCurso { get; set; }
        public int? IdPeriodo { get; set; }
        public int? IdSubperiodo { get; set; }
    }
}