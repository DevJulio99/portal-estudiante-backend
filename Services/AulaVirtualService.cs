using Dapper;
using MyPortalStudent.Domain.Dtos.AulaVirtual;
using MyPortalStudent.Domain.IServices;
using MyPortalStudent.Utils;
using Npgsql;
using System.Text.Json;
 
namespace MyPortalStudent.Services
{
    public class AulaVirtualService : IAulaVirtualService 
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantService _tenantService;

        public AulaVirtualService(IConfiguration configuration, ITenantService tenantService)
        {
            _configuration = configuration;
            _tenantService = tenantService;
        }

        private async Task<NpgsqlConnection> GetConnectionAsync()
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            var connection = new NpgsqlConnection(connectionString);
            await connection.SetTenantIfAvailableAsync(_tenantService);
            return connection;
        }

        public async Task<SilaboResponseDto?> GetSilaboPorCursoAsync(SilaboRequestDto request)
        {
            await using var connection = await GetConnectionAsync();
            var jsonResponse = await connection.QuerySingleAsync<string>("SELECT obtener_silabo_con_sesiones(@CodCurso)", new { CodCurso = request.CodCurso });
            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            };
            return JsonSerializer.Deserialize<SilaboResponseDto>(jsonResponse, options);
        }
    }
}