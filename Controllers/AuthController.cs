using MyPortalStudent.Utils;
using Microsoft.AspNetCore.Mvc;
using MyPortalStudent.Domain;
using System.Security.Claims;
using System.Threading.Tasks;
using MyPortalStudent.Domain.DTOs;
using System.Text.RegularExpressions;

namespace JwtLoginService
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRedisDB _redisDB;

        public AuthController(IAuthService authService, IRedisDB redisDB)
        {
            _authService = authService;
            this._redisDB = redisDB;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            var user = await _authService.ValidateUserAsync(loginRequest);
            var responseValidation = new ApiResult<string>();
            var storedCaptchaCode = _redisDB.GetString($"CaptchaCode_{loginRequest.CaptchaId}");

            if (user == null){
                return this.StatusCode(401, new ApiResult<string>
                {
                    Success = false,
                    Code = ConstantesPortal.ErrorRequest.code4000,
                    Message = ConstantesPortal.ErrorRequest.Message4000,
                    validations = responseValidation.validations
                });
            }

            if (string.IsNullOrEmpty(loginRequest.CaptchaId))
            {
                responseValidation.validations.Add(new ApiValidation
                {
                    Messages = new List<string> { "El campo captchaId es obligatorio." },
                    field = "captchaId"
                });
            }
            if (string.IsNullOrEmpty(loginRequest.CaptchaCode))
            {
                responseValidation.validations.Add(new ApiValidation
                {
                    Messages = new List<string> { "El campo captchaCode es obligatorio." },
                    field = "captchaCode"
                });
            }

            if (!string.IsNullOrEmpty(loginRequest.CaptchaCode) && loginRequest.CaptchaCode.Length != 4)
            {
                responseValidation.validations.Add(new ApiValidation
                {
                    Messages = new List<string> { "El campo captchaCode solo puede contener 4 caracteres." },
                    field = "captchaCode"
                });
            }

            if (!string.IsNullOrEmpty(loginRequest.CaptchaCode) && !Regex.IsMatch(loginRequest.CaptchaCode, "^[a-zA-Z0-9]+$"))
            {
                responseValidation.validations.Add(new ApiValidation
                {
                    Messages = new List<string> { "El campo captchaCode solo puede contener caracteres alfanuméricos." },
                    field = "captchaCode"
                });

            }

            if(storedCaptchaCode != null && storedCaptchaCode != loginRequest.CaptchaCode){
                responseValidation.validations.Add(new ApiValidation
                {
                    Messages = new List<string> { "El campo captchaCode no es valido." },
                    field = "captchaCode"
                });
            }

            if (responseValidation.validations.Count > 0)
            {
                responseValidation.Code = ConstantesPortal.ErrorRequest.code4001;
                responseValidation.Message = ConstantesPortal.ErrorRequest.Message4001;
                return this.StatusCode(400, new ApiResult<string>
                {
                    Success = false,
                    Code = responseValidation.Code,
                    Message = responseValidation.Message,
                    validations = responseValidation.validations
                });
            }

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

        [HttpGet("generar-captcha")]
        public IActionResult GenerateCaptcha()
        {
            int width = 180;
            int height = 50;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var captchaId = Guid.NewGuid().ToString();

            var token = _redisDB.GetString($"CaptchaCode_{captchaId}");
            if (token != null)
            {
                _redisDB.DeleteKey(token);
            }
            _redisDB.SetString($"CaptchaCode_{captchaId}", captchaCode, TimeSpan.FromMinutes(3));

            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode.ToUpper());

            var response = new
            {
                Success = true,
                code = "PS-EVAL",
                Messages = "Se obtuvo la información correctamente",
                Data = new
                {
                    captchaId = captchaId,
                    captchaImage = result.CaptchBase64Data
                }
            };

            return Ok(response);
        }

        [HttpPost("validar-captcha")]
        public IActionResult ValidateCaptcha([FromForm] string? captchaId, [FromForm] string? captchaCode)
        {
            var response = new ApiResult<string>();

            if (string.IsNullOrEmpty(captchaId))
            {
                response.Code = ConstantesPortal.ErrorRequest.code4000;
                response.Message = ConstantesPortal.ErrorRequest.Message4000;
                response.validations.Add(new ApiValidation
                {
                    Messages = ["El campo captchaId es obligatorio."],
                    field = "captchaId"
                });
            }

            if (string.IsNullOrEmpty(captchaCode))
            {
                response.Code = ConstantesPortal.ErrorRequest.code4000;
                response.Message = ConstantesPortal.ErrorRequest.Message4000;
                response.validations.Add(new ApiValidation
                {
                    Messages = ["El campo captchaCode es obligatorio."],
                    field = "captchaCode"
                });
            }

            if (!string.IsNullOrEmpty(captchaCode) && captchaCode.Length != 4)
            {
                response.Code = ConstantesPortal.ErrorRequest.code4000;
                response.Message = ConstantesPortal.ErrorRequest.Message4000;
                response.validations.Add(new ApiValidation
                {
                    Messages = ["El campo captchaCode no es válido."],
                    field = "captchaCode"
                });
            }

            if (response.validations.Count > 0)
            {
                return this.StatusCode(400, new { success = false, code = response.Code, Messages = response.Message, validations = response.validations });
            }

            var storedCaptchaCode = _redisDB.GetString($"CaptchaCode_{captchaId}");

            if (string.IsNullOrEmpty(storedCaptchaCode))
            {
                return Ok(new ApiResult<string>
                {
                    Success = false,
                    Code = ConstantesPortal.ErrorRequest.code4002,
                    Message = ConstantesPortal.ErrorRequest.Message4002
                });
            }

            if (storedCaptchaCode != null && storedCaptchaCode == captchaCode.ToUpper())
            {
                return Ok(new ApiResult<string> { Success = true, Code = ConstantesPortal.Success.code20016, Message = ConstantesPortal.Success.Message20016 });
            }

            return Ok(new ApiResult<string> { Success = false, Code = ConstantesPortal.ErrorRequest.code4001, Message = ConstantesPortal.ErrorRequest.Message4001 });
        }
    }
}
