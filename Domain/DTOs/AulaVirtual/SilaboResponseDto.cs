using System.Text.Json.Serialization;

namespace MyPortalStudent.Domain.Dtos.AulaVirtual
{
    public class SilaboResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public List<UnidadDto>? Data { get; set; }
    }

    public class UnidadDto
    {
        [JsonPropertyName("id_unidad")]
        public int IdUnidad { get; set; }

        [JsonPropertyName("unidad_titulo")]
        public string? UnidadTitulo { get; set; }

        [JsonPropertyName("fecha_inicio")]
        public DateTime? FechaInicio { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTime? FechaFin { get; set; }

        [JsonPropertyName("sesiones")]
        public List<SesionDto>? Sesiones { get; set; }
    }

    public class SesionDto
    {
        [JsonPropertyName("numero")]
        public int Numero { get; set; }

        [JsonPropertyName("titulo")]
        public string? Titulo { get; set; }

        [JsonPropertyName("fecha")]
        public DateTime? Fecha { get; set; }

        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }

        [JsonPropertyName("contenido_sesion")]
        public List<ContenidoSesionDto>? ContenidoSesion { get; set; }
    }

    public class ContenidoSesionDto
    {
        [JsonPropertyName("tipo")]
        public string? Tipo { get; set; }

        [JsonPropertyName("titulo")]
        public string? Titulo { get; set; }

        [JsonPropertyName("contenido_hijo")]
        public List<ContenidoHijoDto>? ContenidoHijo { get; set; }
    }

    public class ContenidoHijoDto
    {
        [JsonPropertyName("tipo")]
        public string? Tipo { get; set; }

        [JsonPropertyName("titulo")]
        public string? Titulo { get; set; }

        [JsonPropertyName("fecha_apertura")]
        public DateTime? FechaApertura { get; set; }

        [JsonPropertyName("fecha_cierre")]
        public DateTime? FechaCierre { get; set; }

        [JsonPropertyName("tiene_material")]
        public Boolean TieneMaterial { get; set; }
    }
}