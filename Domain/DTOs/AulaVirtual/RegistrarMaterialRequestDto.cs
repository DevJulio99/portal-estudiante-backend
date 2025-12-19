namespace MyPortalStudent.Domain.Dtos.AulaVirtual
{
    public class RegistrarMaterialRequestDto
    {
        public int IdContenido { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public decimal PesoMb { get; set; }
        public int IdAlumno { get; set; }
    }
}