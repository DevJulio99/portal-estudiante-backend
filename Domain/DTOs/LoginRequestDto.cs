
    namespace MyPortalStudent.Domain
{
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? CaptchaId { get; set; }
        public string? CaptchaCode { get; set; }
    }
    
}
