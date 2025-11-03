using System.Text.Json.Serialization;

namespace MyPortalStudent.Domain.DTOs.Notas
{
    public class NotaDetalleDto
    {
        [JsonPropertyName("id_nota")] public int IdNota { get; set; }
        [JsonPropertyName("tipo_nota")] public string TipoNota { get; set; } = string.Empty;
        [JsonPropertyName("nota")] public decimal Nota { get; set; }
        [JsonPropertyName("peso")] public decimal Peso { get; set; }
    }
}