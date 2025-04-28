namespace MyPortalStudent.Domain {
public class CursoActualizarDTO
{
    public required int IdCurso { get; set; }
    public required string DescripcionCurso { get; set; }
    public required decimal Creditos { get; set; }
    public required string Modalidad { get; set; }
    public required string Nivel { get; set; }
    public required string CodigoSede { get; set; }
}
}
