namespace MyPortalStudent.Domain {
public class AlumnoAsistenciaDTO
{
    public required int idAsistencia { get; set; }
    public required string dia { get; set; }
    public required string estadoAsistencia { get; set; }
    public required string descripcionCurso { get; set; }
    public required string modalidad { get; set; }
}
}
