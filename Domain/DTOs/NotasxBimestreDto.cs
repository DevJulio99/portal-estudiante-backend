namespace MyPortalStudent.Domain
{
    public class NotasxBimestreDTO
    {
        public required string alumno { get; set; }
        public required string apellidoPaterno { get; set; }
        public required string apellidoMaterno { get; set; }
        public required string descripcionCurso { get; set; }
        public required string codigoPeriodo { get; set; }
        public required string descripcionPeriodo { get; set; }
        public required string nota { get; set; }
        public required string peso { get; set; }
        public required string tipoNota { get; set; }
    }
}
