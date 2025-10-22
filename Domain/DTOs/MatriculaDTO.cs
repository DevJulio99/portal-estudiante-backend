namespace MyPortalStudent.Domain.DTOs
{
    public class MatriculaDTO
    {
        public required int IdMatricula { get; set; }
        public required int IdAlumno { get; set; }
        public required string FechaInicio { get; set; }
        public required string FechaFin { get; set; }
        public required string TipoMatricula { get; set; }
        public required string EstadoMatricula { get; set; }
        public int? IdSeccion { get; set; }
        public string? Observaciones { get; set; }
        public int? Veces { get; set; }
        public required int IdPeriodo { get; set; }
        public required int IdGrado { get; set; }
        public required string CodigoSede { get; set; }
        public required string FechaMatricula { get; set; }
        public required string UsuarioRegistro { get; set; }
        public required bool Activo { get; set; }
        
        // Información adicional del alumno
        public string? NombreAlumno { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? DniAlumno { get; set; }
        
        // Información adicional del grado
        public string? DescripcionGrado { get; set; }
        public string? NivelEducativo { get; set; }
        
        // Información adicional del período
        public string? DescripcionPeriodo { get; set; }
        public string? CodigoPeriodo { get; set; }
        
        // Información adicional de la sede
        public string? DescripcionSede { get; set; }
    }
}
