namespace MyPortalStudent.Domain {
public class AsistenciaCursoAlumnoDTO
{
    public required int idAlumno { get; set; }
    public required int anio { get; set; }
    public required DateTime inicioPeriodo { get; set; }
    public required DateTime finalPeriodo { get; set; }
    public required string codigoCurso { get; set; }
    public required string estadoAsistencia { get; set; }
}
}
