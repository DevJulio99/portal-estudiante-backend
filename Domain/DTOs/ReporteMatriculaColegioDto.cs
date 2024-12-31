namespace MyPortalStudent.Domain
{
    public class ReporteMatriculaColegioDTO
    {
        public required string modalidad { get; set; }
        public required string codCurso { get; set; }
        public required string descCurso { get; set; }
        public required string periodo { get; set; }
        public required string salon { get; set; }
        public required string seccion { get; set; }
        public required DocenteCursoDTO[] docente { get; set; }
        public required string ciclo { get; set; }
        public required string creditos { get; set; }
        public required string cantidadVeces { get; set; }
        public required string inasistencias { get; set; }
        public required string statusCurso { get; set; }
        public required int orden { get; set; }
        public required int notaFinal { get; set; }
        public required Boolean tieneHorario { get; set; }
        public string periodoAcademico { get; set; }
        public string nivel { get; set; }
        public string fechaInicio { get; set; }
        public string fechaFin { get; set; }
    }
}
