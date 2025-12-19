using System.Text.Json.Serialization;

namespace MyPortalStudent.Domain.Dtos.AulaVirtual
{
    public class MaterialesResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public List<ArchivoContenidoDto>? Data { get; set; }
    }

    public class ArchivoContenidoDto
    {
        [JsonPropertyName("id_archivo")]
        public int IdArchivo { get; set; }

        [JsonPropertyName("id_contenido")]
        public int IdContenido { get; set; }

        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("extension")]
        public string? Extension { get; set; }
    }
}