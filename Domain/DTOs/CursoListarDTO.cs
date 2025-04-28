namespace MyPortalStudent.Domain {
public class CursoListarDTO
{
    public required int IdCurso { get; set; }
    public required string CodigoCurso { get; set; }
    public required string Descripcion { get; set; }
    public required decimal Creditos { get; set; }
    public required string Modalidad { get; set; }
    public required string Nivel { get; set; }
    public int Total { get; set; }
}
}

