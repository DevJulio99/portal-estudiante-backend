namespace MyPortalStudent.Domain.DTOs
{
    public class BaseResponseDTO
    {
        public required bool Success { get; set; }
        public required string Message { get; set; }
    }
}