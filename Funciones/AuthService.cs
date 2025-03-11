using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using JwtLoginService;
using Microsoft.IdentityModel.Tokens;
using MyPortalStudent.Domain;
using Npgsql;

namespace APIPostulaEnrolamiento.Funciones
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<UserDto> ValidateUserAsync(LoginRequestDto loginRequest)
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            const string query = @"SELECT Id, Email, Name, Phone, Dni_Usuario, Role, Codigo_Sede, s.Tipo_Institucion, (select id_alumno from alumno where dni = u.Dni_Usuario) as Id_Alumno
             FROM Users u INNER JOIN Sede s USING(Codigo_Sede)
             WHERE Email = @Email AND Password = @Password AND Activo = true";
            return await connection.QueryFirstOrDefaultAsync<UserDto>(query, new { loginRequest.Email, loginRequest.Password });
        }

        public string GenerateJwtToken(UserDto user, int expirationHours)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Email", user.Email),
                new Claim("Name", user.Name),
                new Claim("Phone", user.Phone),
                new Claim("Dni_Usuario", user.Dni_Usuario),
                new Claim("Role", user.Role),
                new Claim("Codigo_Sede", user.Codigo_Sede ?? ""),
                new Claim("Id_Alumno", user.Id_Alumno ?? "0"),
                new Claim("Tipo_Institucion", user.Tipo_Institucion ?? "0")
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(expirationHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }

}