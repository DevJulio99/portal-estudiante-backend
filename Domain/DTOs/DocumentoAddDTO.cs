using System.Text.Json.Serialization;

public class DocumentoAddDTO
{
    public int IdCategoriaDocumento { get; set; }
    [JsonIgnore] public string? Status { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    [JsonIgnore] public string? Enlace { get; set; }
    [JsonIgnore] public int? Secuencia { get; set; }
    [JsonIgnore] public DateTime? DateCreated { get; set; }
    [JsonIgnore] public string? TipoDocumento { get; set; }
    [JsonIgnore] public bool MasBuscados { get; set; }
    [JsonIgnore] public int? SecuenciaMasBuscada { get; set; }
    public string Documento { get; set; }
    public bool Interno { get; set; }
    [JsonIgnore] public DateTime? FechaActualizacion { get; set; }
    [JsonIgnore] public DateTime? FechaInicio { get; set; }
    [JsonIgnore] public DateTime? FechaFin { get; set; }
    public string DocumentoDescarga { get; set; }
    [JsonIgnore] public string? NombreDocumento { get; set; }
    [JsonIgnore] public string? Type { get; set; }
}