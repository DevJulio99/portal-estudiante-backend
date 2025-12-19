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
            var jsonResponse = await connection.QuerySingleAsync<string>("SELECT obtener_silabo_con_sesiones(@CodCurso, @p_id_alumno)", new { CodCurso = request.CodCurso, p_id_alumno = request.IdAlumno });
            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            };
            return JsonSerializer.Deserialize<SilaboResponseDto>(jsonResponse, options);
        }

        public async Task<MaterialesResponseDto?> GetMateriales(MaterialesRequestDto request)
        {
            await using var connection = await GetConnectionAsync();
            var jsonResponse = await connection.QuerySingleAsync<string>("SELECT obtener_archivo_contenido(@p_id_contenido, @p_id_alumno)", new { p_id_contenido = request.IdContenido, p_id_alumno = request.IdAlumno });
            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            };
            return JsonSerializer.Deserialize<MaterialesResponseDto>(jsonResponse, options);
        }

        public async Task<RegistrarMaterialResponseDto?> RegistrarMaterial(RegistrarMaterialRequestDto request)
        {
            await using var connection = await GetConnectionAsync();
            var jsonResponse = await connection.QuerySingleAsync<string>(
                "SELECT insertar_archivo_contenido(@p_id_contenido, @p_nombre, @p_url, @p_extension, @p_peso_mb, @p_id_alumno)", 
                new { 
                    p_id_contenido = request.IdContenido, 
                    p_nombre = request.Nombre, 
                    p_url = request.Url, 
                    p_extension = request.Extension, 
                    p_peso_mb = request.PesoMb, 
                    p_id_alumno = request.IdAlumno 
                });
            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            };
            return JsonSerializer.Deserialize<RegistrarMaterialResponseDto>(jsonResponse, options);
        }
    }
}