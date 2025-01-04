namespace MyPortalStudent.Domain
{
    public class HorarioCursoxDocenteDTO
    {
        public required string descripcionCurso { get; set; }
        public required string nombreDia { get; set; }
        public required string horaInicio { get; set; }
        public required string horaFin { get; set; }
        public required string descripcionSeccion { get; set; }
        public required string nombreDocente { get; set; }
        public required string apellidoPaterno { get; set; }
        public required string apellidoMaterno { get; set; }
    }
}
