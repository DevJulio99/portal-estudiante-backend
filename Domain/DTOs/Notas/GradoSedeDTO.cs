namespace MyPortalStudent.Domain.DTOs.Notas
{
    public class GradoSedeDTO
    {
        public int IdGrado { get; set; }
        public string DescripcionGrado { get; set; } = string.Empty;
        public string NivelEducativo { get; set; } = string.Empty;
        public string TipoInstitucion { get; set; } = string.Empty;
    }
}