namespace MyPortalStudent.Domain.DTOs.Notas
{
    public class SeccionesRequestDTO
    {
        public string? CodSede { get; set; }
        public int? IdGrado { get; set; }
        public string? TipoInstitucion { get; set; }
        public int? IdCiclo { get; set; }
    }
}