namespace MyPortalStudent.Domain.DTOs.Notas
{
    public class CursosPorGradoSedeRequestDTO
    {
        public string? CodSede { get; set; }
        public string? TipoInstitucion { get; set; }
        public int? IdGrado { get; set; }
    }
}