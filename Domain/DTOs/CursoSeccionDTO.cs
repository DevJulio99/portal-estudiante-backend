using System.Text.Json.Serialization;

namespace MyPortalStudent.Domain.DTOs
{
    public class CursoSeccionDTO
    {
        [JsonPropertyName("id_curso")]
        public int IdCurso { get; set; } = 0;
        [JsonPropertyName("codigo_curso")]
        public string CodigoCurso { get; set; } = string.Empty;

        [JsonPropertyName("descripcion_curso")]
        public string DescripcionCurso { get; set; } = string.Empty;

        [JsonPropertyName("secciones")]
        public List<SeccionInfoDTO> Secciones { get; set; } = [];
    }

    public class SeccionInfoDTO
    {
        [JsonPropertyName("id_seccion")]
        public int IdSeccion { get; set; } = 0;
        [JsonPropertyName("codigo_seccion")]
        public string CodigoSeccion { get; set; } = string.Empty;

        [JsonPropertyName("descripcion_seccion")]
        public string DescripcionSeccion { get; set; } = string.Empty;

        [JsonPropertyName("horario")]
        public HorarioInfoDTO? Horario { get; set; }
    }

    public class HorarioInfoDTO
    {
        [JsonPropertyName("turno")]
        public string? Turno { get; set; }
        [JsonPropertyName("nombre_dia")]
        public string? NombreDia { get; set; }
        [JsonPropertyName("fecha_inicio")]
        public string? FechaInicio { get; set; }
        [JsonPropertyName("fecha_fin")]
        public string? FechaFin { get; set; }
        [JsonPropertyName("hora_inicio")]
        public string? HoraInicio { get; set; }
        [JsonPropertyName("hora_fin")]
        public string? HoraFin { get; set; }
    }
}
