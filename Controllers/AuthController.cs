using Microsoft.AspNetCore.Mvc;
using MyPortalStudent.Domain;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JwtLoginService
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            var user = await _authService.ValidateUserAsync(loginRequest);

            if (user == null)
                return Unauthorized();

            var accessToken = _authService.GenerateJwtToken(user, 1); // 1 hour expiration
            var refreshToken = _authService.GenerateJwtToken(user, 24); // 24 hour expiration

            return Ok(new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh-token")]
        public ActionResult<TokenResponseDto> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var principal = _authService.GetPrincipalFromExpiredToken(refreshTokenDto.RefreshToken);
            if (principal == null)
                return Unauthorized();

            var user = new UserDto
            {
                Email = principal.FindFirstValue("Email"),
                Name = principal.FindFirstValue("Name"),
                Phone = principal.FindFirstValue("Phone"),
                Role = principal.FindFirstValue("Role")
            };

            var newAccessToken = _authService.GenerateJwtToken(user, 1);
            var newRefreshToken = _authService.GenerateJwtToken(user, 24);

            return Ok(new TokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}
