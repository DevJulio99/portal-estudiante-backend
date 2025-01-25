namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class PostulanteDTO
    {
        public int? idPostulante { get; set; }
        public string dni { get; set; }
        public string nombre { get; set; } = null!;
        public string apellido { get; set; } = null!;
        public string correo { get; set; } = null!;
        public string? celular { get; set; }
        public int? codigoPostulante { get; set; }
        public Boolean estado { get; set; }
    }
}