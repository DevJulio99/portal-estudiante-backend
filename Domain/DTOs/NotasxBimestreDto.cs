namespace MyPortalStudent.Domain
{
    public class NotasxBimestreDTO
    {
        public required string alumno { get; set; }
        public required string ApellidoPaterno { get; set; }
        public required string ApellidoMaterno { get; set; }
        public required string CodigoCurso { get; set; }
        public required string DescripcionCurso { get; set; }
        public required string CodigoPeriodo { get; set; }
        public required string DescripcionPeriodo { get; set; }
        public required string CodigoSubperiodo { get; set; }
        public decimal nota { get; set; }
        public decimal peso { get; set; }
        public required string tipoNota { get; set; }
    }
}
