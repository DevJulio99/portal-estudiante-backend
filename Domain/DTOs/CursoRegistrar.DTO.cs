namespace MyPortalStudent.Domain {
public class CursoRegistrarDTO
{
    public required string DescripcionCurso { get; set; }
    public required decimal Creditos { get; set; }
    public required string Modalidad { get; set; }
    public required string Nivel { get; set; }
    public required string CodigoSede { get; set; }
}
}
