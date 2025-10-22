namespace MyPortalStudent.Domain.DTOs
{
    public class MatriculaListarDTO
    {
        public required int IdMatricula { get; set; }
        public required string NombreAlumno { get; set; }
        public required string ApellidoPaterno { get; set; }
        public required string ApellidoMaterno { get; set; }
        public required string Dni { get; set; }
        public required string DescripcionGrado { get; set; }
        public required string EstadoMatricula { get; set; }
        public required string FechaMatricula { get; set; }
        public required string CodigoSede { get; set; }
        public required string DescripcionPeriodo { get; set; }
    }
}
