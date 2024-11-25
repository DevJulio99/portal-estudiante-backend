namespace MyPortalStudent.Domain
{
    public class AsistenciaDTO
    {
        public required int idAsistencia { get; set; }
        public required string dia { get; set; }
        public required string estadoAsistencia { get; set; }
        public required int idAlumno { get; set; }
        public required int idCurso { get; set; }
    }
}
