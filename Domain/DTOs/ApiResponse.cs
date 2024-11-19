using System.Text.Json.Serialization;

namespace MyPortalStudent.Domain
{
    public class ApiResponse<T>
    {
        public ApiResponse()
        {
            this.Success = true;
        }

        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;
        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }
}
