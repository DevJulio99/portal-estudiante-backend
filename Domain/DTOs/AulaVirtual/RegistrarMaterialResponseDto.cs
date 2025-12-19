using System.Text.Json.Serialization;

namespace MyPortalStudent.Domain.Dtos.AulaVirtual
{
    public class RegistrarMaterialResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public RegistrarMaterialDataDto? Data { get; set; }
    }

    public class RegistrarMaterialDataDto
    {
        [JsonPropertyName("id_archivo")]
        public int IdArchivo { get; set; }

        [JsonPropertyName("id_contenido")]
        public int IdContenido { get; set; }

        [JsonPropertyName("id_alumno")]
        public int IdAlumno { get; set; }
    }
}