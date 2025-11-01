namespace MyPortalStudent.Domain.DTOs.Notas
{
    public class AlumnosPorFiltroRequestDTO
    {
        public int IdPeriodo { get; set; }
        public int? IdSubperiodo { get; set; }
        public string CodSede { get; set; } = string.Empty;
        public int IdGrado { get; set; }
        public int IdSeccion { get; set; }
        public int IdCurso { get; set; }
    }
}