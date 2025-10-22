namespace MyPortalStudent.Domain.DTOs
{
    public class MatriculaResponseDTO
    {
        public required bool Success { get; set; }
        public required string Message { get; set; }
        public int? IdMatricula { get; set; }
        public string? FechaMatricula { get; set; }
    }
}
