public class CategoriaDocumentoDTO
{
    public int Id { get; set; }
    public string Status { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public string Imagen { get; set; }
    public int Secuencia { get; set; }
    public DateTime DateCreated { get; set; }
    public List<DocumentoDTO> Documentos { get; set; }
}