using System.ComponentModel.DataAnnotations;

namespace MyPortalStudent.Domain.DTOs
{
    public class CursosPorGradoRequestDTO
    {
        [Required]
        public int IdGrado { get; set; }
        [Required]
        public string TipoInstitucion { get; set; } = string.Empty;
    }
}
