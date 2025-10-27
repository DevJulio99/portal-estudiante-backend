namespace MyPortalStudent.Domain.DTOs
{
    public class MatriculaRegistrarDTO
    {
        public required int IdAlumno { get; set; }
        public required int IdPeriodo { get; set; }
        public int? IdGrado { get; set; }
        public required string CodigoSede { get; set; }
        public string TipoMatricula { get; set; } = "Regular";
        public string EstadoMatricula { get; set; } = "Activa";
        public string? Observaciones { get; set; }
        public string UsuarioRegistro { get; set; } = "SISTEMA";
        public string TipoInstitucion { get; set; } = "";
    }
}
