namespace MyPortalStudent.Domain.DTOs
{
    public class MatriculaDTO
    {
        public int IdMatricula { get; set; }
        public int IdAlumno { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string TipoMatricula { get; set; }
        public string EstadoMatricula { get; set; }
        public int? IdSeccion { get; set; }
        public string? Observaciones { get; set; }
        public int? Veces { get; set; }
        public int IdPeriodo { get; set; }
        public int IdGrado { get; set; }
        public string CodigoSede { get; set; }
        public string FechaMatricula { get; set; }
        public string UsuarioRegistro { get; set; }
        public bool Activo { get; set; }
        
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
