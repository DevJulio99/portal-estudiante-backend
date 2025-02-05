public class DocumentoDTO
{
    public int Id { get; set; }
    public string Status { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public string Enlace { get; set; }
    public int Secuencia { get; set; }
    public DateTime DateCreated { get; set; }
    public string TipoDocumento { get; set; }
    public bool MasBuscados { get; set; }
    public int? SecuenciaMasBuscada { get; set; }
    public string Documento { get; set; }
    public bool Interno { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string DocumentoDescarga { get; set; }
    public string NombreDocumento { get; set; }
    public string Type { get; set; }
}