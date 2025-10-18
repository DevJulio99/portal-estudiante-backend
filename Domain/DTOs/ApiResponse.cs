using System.Text.Json.Serialization;

namespace MyPortalStudent.Domain
{
    public class ErrorDetails
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
    }

    public class ApiResponse<T>
    {
        public ApiResponse()
        {
            Success = true;
            Flag = true;
        }

        [JsonPropertyName("flag")]
        public bool Flag { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("error")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ErrorDetails? Error { get; set; }
    }
}
