namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class RegistrarPostulanteDTO
    {
        public string dni { get; set; }
        public string nombre { get; set; } = null!;
        public string apellido { get; set; } = null!;
        public string correo { get; set; } = null!;
        public Boolean estado { get; set; }
    }
}