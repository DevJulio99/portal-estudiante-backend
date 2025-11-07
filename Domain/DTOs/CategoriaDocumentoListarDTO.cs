using System.Text.Json.Serialization;

namespace MyPortalStudent.Domain.DTOs
{
    public class CategoriaDocumentoListarDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }
        [JsonPropertyName("secuencia")]
        public int Secuencia { get; set; }
    }
}