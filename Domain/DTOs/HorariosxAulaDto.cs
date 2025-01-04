namespace MyPortalStudent.Domain
{
    public class HorarioxAulaDTO
    {
        public required string descripcionAula { get; set; }
        public required string descripcionCurso { get; set; }
        public required string nombreDia { get; set; }
        public required string horaInicio { get; set; }
        public required string horaFin { get; set; }
        public required string seccion { get; set; }
        public required string nombreDocente { get; set; }
        public required string apellidoPaternoDocente { get; set; }
        public required string apellidoMaternoDocente { get; set; }
    }
}
