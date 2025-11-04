using System.Security.Claims;
using MyPortalStudent.Domain;

namespace MyPortalStudent.Domain.Ifunciones
{
    public interface IAuthService
    {
        Task<UserDto> ValidateUserAsync(LoginRequestDto loginRequest);
        string GenerateJwtToken(UserDto user, int expirationHours);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
