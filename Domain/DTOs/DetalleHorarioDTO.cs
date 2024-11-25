namespace MyPortalStudent.Domain
{
    public class DetalleHorarioDTO
    {
        public required string titulo { get; set; }
        public required string nrc { get; set; }
        public required string descripMetodoEducativo { get; set; }
        public required string codmetodoEducativo { get; set; }
        public required string descripMateria { get; set; }
        public required string codMateria { get; set; }
        public required string codSeccion { get; set; }
        public required string descripPartePeriodo { get; set; }
        public required string codPartePeriodo { get; set; }
        public required string cantidadVeces { get; set; }
        public required string codAula { get; set; }
        public required string descripAula { get; set; }
        public required string codCampus { get; set; }
        public required string descripCampus { get; set; }
        public required string codEdificio { get; set; }
        public required string descripEdificio { get; set; }
        public required string fechaInicio { get; set; }
        public required string fechaFin { get; set; }
        public required string diaNombre { get; set; }
        public required string diaNumero { get; set; }
        public required string horaInicio { get; set; }
        public required string horaFin { get; set; }
        public required ProfesorDTO[] profesor { get; set; }
        public required string orden { get; set; }
        public required string codCurso { get; set; }
    }
}
