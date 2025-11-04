using Dapper;
using Microsoft.Extensions.Configuration;
using MyPortalStudent.Domain.DTOs;
using MyPortalStudent.Domain.DTOs.Notas;
using MyPortalStudent.Domain.IServices;
using MyPortalStudent.Utils;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Text.Json;

namespace MyPortalStudent.Services
{
    public class NotasService : INotasService
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantService? _tenantService;

        public NotasService(IConfiguration configuration, ITenantService? tenantService = null)
        {
            _configuration = configuration;
            _tenantService = tenantService;
        }

        /// <summary>
        /// Crea una conexión y establece el tenant automáticamente
        /// </summary>
        private async Task<NpgsqlConnection> GetConnectionAsync()
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            var connection = new NpgsqlConnection(connectionString);
            await connection.SetTenantIfAvailableAsync(_tenantService);
            return connection;
        }

        public async Task<List<PeriodoSedeDTO>> GetPeriodoPorSede(PeriodoRequestDTO request)
        {
            await using var connection = await GetConnectionAsync();
            const string sql = @"SELECT DISTINCT 
                                    p.id_periodo,
                                    p.descripcion_periodo
                                FROM periodoacademico p
                                JOIN matricula m ON m.id_periodo = p.id_periodo
                                JOIN sede s ON m.codigo_sede = s.codigo_sede
                                WHERE 
                                    s.codigo_sede = @codSede
                                    AND (
                                        (s.tipo_institucion ilike 'C' AND p.tipo_periodo = 'Año')
                                        OR
                                        (s.tipo_institucion ilike 'I' AND p.tipo_periodo = 'Ciclo')
                                    )
                                ORDER BY p.descripcion_periodo DESC;";

            var periodos = await connection.QueryAsync<PeriodoSedeDTO>(sql, new { codSede = request.CodSede });
            return periodos.AsList();
        }

        public async Task<List<GradoSedeDTO>> GetGradoPorSede(GradoRequestDTO request)
        {
            await using var connection = await GetConnectionAsync();
            const string sql = @"SELECT DISTINCT 
                                    g.""ID_GRADO"",
                                    g.""DESCRIPCION_GRADO"",
                                    g.""NIVEL_EDUCATIVO"",
                                    g.tipo_institucion
                                FROM grado g
                                JOIN matricula m ON m.id_grado = g.""ID_GRADO""
                                JOIN sede s ON m.codigo_sede = s.codigo_sede
                                WHERE s.codigo_sede = @codSede and s.tipo_institucion ilike g.tipo_institucion
                                ORDER BY g.""NIVEL_EDUCATIVO"", g.""DESCRIPCION_GRADO"";";

            var grados = await connection.QueryAsync<GradoSedeDTO>(sql, new { codSede = request.CodSede });
            return grados.AsList();
        }

        public async Task<List<SubperiodoDTO>> GetSubperiodosPorPeriodo(SubperiodoRequestDTO request)
        {
            await using var connection = await GetConnectionAsync();
            const string sql = @"SELECT 
                                    sp.id_subperiodo,
                                    sp.descripcion_subperiodo
                                FROM subperiodos sp
                                WHERE sp.id_periodo = @IdPeriodo
                                ORDER BY sp.fecha_inicio;";
            var subperiodos = await connection.QueryAsync<SubperiodoDTO>(sql, new { IdPeriodo = request.IdPeriodo });
            return subperiodos.AsList();
        }

        public async Task<List<SeccionGradoDTO>> GetSeccionesPorGrado(SeccionesRequestDTO request)
        {
            await using var connection = await GetConnectionAsync();
            const string sql = @"SELECT DISTINCT
    se.id_seccion,
    se.descripcion AS descripcion_seccion
FROM seccion se
JOIN matricula m ON se.id_seccion = m.id_seccion
JOIN sede s ON m.codigo_sede = s.codigo_sede
WHERE 
    (@Sede IS NULL OR s.codigo_sede = @Sede)
    AND (@TipoInstitucion IS NULL OR s.tipo_institucion ILIKE @TipoInstitucion)
    AND (
        (s.tipo_institucion ILIKE 'C' AND (@Grado IS NULL OR se.grado = @Grado) AND se.ciclo IS NULL)
        OR
        (s.tipo_institucion ILIKE 'I' AND (@Ciclo IS NULL OR se.ciclo = @Ciclo) AND se.grado IS NULL)
    )
ORDER BY se.descripcion;
";

            var parameters = new
            {
                Sede = request.CodSede,
                Grado = request.IdGrado?.ToString(),
                TipoInstitucion = request.TipoInstitucion,
                Ciclo = request.IdCiclo?.ToString()
            };
            var secciones = await connection.QueryAsync<SeccionGradoDTO>(sql, parameters);
            return secciones.AsList();
        }

        public async Task<List<CursoGradoDTO>> GetCursosPorGrado(CursosPorGradoSedeRequestDTO request)
        {
            await using var connection = await GetConnectionAsync();
            const string sql = @"SELECT DISTINCT
                                    c.id_curso,
                                    c.descripcion_curso
                                FROM curso c
                                JOIN grado_curso gc ON c.id_curso = gc.id_curso
                                JOIN matricula_curso mc ON mc.id_curso = c.id_curso
                                JOIN matricula m ON mc.id_matricula = m.id_matricula
                                JOIN sede s ON m.codigo_sede = s.codigo_sede
                                WHERE 
                                    (@codSede IS NULL OR s.codigo_sede = @codSede)
                                    AND (@tipoInstitucion IS NULL OR s.tipo_institucion ILIKE @tipoInstitucion)
                                    AND (@idGrado IS NULL OR gc.""ID_GRADO"" = @idGrado)
                                ORDER BY c.descripcion_curso;";

            var cursos = await connection.QueryAsync<CursoGradoDTO>(sql, request);
            return cursos.AsList();
        }

        public async Task<List<AlumnoFiltroDTO>> GetAlumnosPorFiltro(AlumnosPorFiltroRequestDTO request)
        {
            await using var connection = await GetConnectionAsync();
            const string sql = @"SELECT DISTINCT
                                    a.id_alumno,
                                    a.codigo_alumno,
                                    a.nombre || ' ' || a.apellido_paterno || ' ' ||  a.apellido_materno as nombreAlumno
                                FROM alumno a
                                JOIN matricula m ON a.id_alumno = m.id_alumno
                                JOIN sede s ON m.codigo_sede = s.codigo_sede
                                JOIN matricula_curso mc ON m.id_matricula = mc.id_matricula
                                JOIN grado_curso gc ON mc.id_curso = gc.id_curso AND gc.""ID_GRADO"" = m.id_grado
                                JOIN curso c ON gc.id_curso = c.id_curso
                                JOIN seccion se ON mc.id_seccion = se.id_seccion
                                LEFT JOIN notas n ON a.id_alumno = n.id_alumno 
                                                  AND n.id_curso = c.id_curso
                                                  AND n.id_periodo = @IdPeriodo
                                                  AND (@IdSubperiodo IS NULL OR n.id_subperiodo = @IdSubperiodo)
                                WHERE 
                                    s.codigo_sede = @CodSede
                                    AND m.id_grado = @IdGrado
                                    AND mc.id_seccion = @IdSeccion
                                    AND c.id_curso = @IdCurso
                                ORDER BY nombreAlumno;";
            var alumnos = await connection.QueryAsync<AlumnoFiltroDTO>(sql, request);
            return alumnos.AsList();
        }

        public async Task<List<NotasAlumnoDTO>> GetNotasAlumno(NotasAlumnoRequestDTO request)
        {
            await using var connection = await GetConnectionAsync();
            const string sql = @"SELECT 
                                    n.id_nota,
                                    n.nota,
                                    n.peso,
                                    n.tipo_nota,
                                    n.id_alumno
                                FROM notas n
                                WHERE 
                                    (@IdAlumno IS NULL OR n.id_alumno = @IdAlumno)
                                    AND (@IdCurso IS NULL OR n.id_curso = @IdCurso)
                                    AND (@IdPeriodo IS NULL OR n.id_periodo = @IdPeriodo)
                                    AND (@IdSubperiodo IS NULL OR n.id_subperiodo = @IdSubperiodo)
                                ORDER BY n.id_periodo, n.id_subperiodo, n.tipo_nota;";
            var notas = await connection.QueryAsync<NotasAlumnoDTO>(sql, request);
            return notas.AsList();
        }

        public async Task<BaseResponseDTO> RegistrarNotasAlumno(RegistrarNotaDto request)
        {
            try
            {
                await using var connection = await GetConnectionAsync();

                var parameters = new
                {
                    p_id_alumno = request.IdAlumno,
                    p_id_curso = request.IdCurso,
                    p_id_periodo = request.IdPeriodo,
                    p_id_subperiodo = request.IdSubperiodo,
                    p_notas = JsonSerializer.Serialize(request.Notas)
                };

                const string sql = "SELECT insertar_notas(@p_id_alumno, @p_id_curso, @p_id_periodo, @p_notas::json, @p_id_subperiodo)";

                var jsonResult = await connection.QuerySingleOrDefaultAsync<string>(sql, parameters, commandType: CommandType.Text);
                
                if (string.IsNullOrEmpty(jsonResult))
                {
                    throw new InvalidOperationException("La función de base de datos para registrar notas no devolvió un resultado.");
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbResult = JsonSerializer.Deserialize<BaseResponseDTO>(jsonResult, options);

                if (dbResult == null || !dbResult.Success)
                {
                    throw new InvalidOperationException(dbResult?.Message ?? "Error al procesar la respuesta de la base de datos.");
                }

                return dbResult;
            }
            catch (PostgresException ex)
            {
                return new BaseResponseDTO { Success = false, Message = ex.MessageText };
            }
        }

        public async Task<BaseResponseDTO> ActualizarNotasAlumno(RegistrarNotaDto request)
        {
            try
            {
                await using var connection = await GetConnectionAsync();

                var parameters = new
                {
                    p_id_alumno = request.IdAlumno,
                    p_id_curso = request.IdCurso,
                    p_id_periodo = request.IdPeriodo,
                    p_id_subperiodo = request.IdSubperiodo,
                    p_notas = JsonSerializer.Serialize(request.Notas)
                };

                const string sql = "SELECT actualizar_notas(@p_id_alumno, @p_id_curso, @p_id_periodo, @p_notas::json, @p_id_subperiodo)";

                var jsonResult = await connection.QuerySingleOrDefaultAsync<string>(sql, parameters, commandType: CommandType.Text);

                if (string.IsNullOrEmpty(jsonResult))
                {
                    throw new InvalidOperationException("La función de base de datos para actualizar notas no devolvió un resultado.");
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbResult = JsonSerializer.Deserialize<BaseResponseDTO>(jsonResult, options);

                if (dbResult == null || !dbResult.Success)
                {
                    throw new InvalidOperationException(dbResult?.Message ?? "Error al procesar la respuesta de la base de datos.");
                }

                return dbResult;
            }
            catch (PostgresException ex)
            {
                return new BaseResponseDTO { Success = false, Message = ex.MessageText };
            }
        }
    }
}