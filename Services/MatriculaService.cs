using Dapper;
using MyPortalStudent.Domain.DTOs;
using MyPortalStudent.Domain.IServices;
using MyPortalStudent.Utils;
using Npgsql;
using System.Data;
using NpgsqlTypes; // Añadir esta importación
using System.Text.Json;

namespace MyPortalStudent.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantService? _tenantService;

        public MatriculaService(IConfiguration configuration, ITenantService? tenantService = null)
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

        public async Task<MatriculaResponseDTO> RealizarMatricula(MatriculaRegistrarDTO matriculaDto)
        {
            await using var connection = await GetConnectionAsync();

            var parameters = new
            {
                p_id_alumno = matriculaDto.IdAlumno,
                p_id_periodo = matriculaDto.IdPeriodo,
                p_id_grado = matriculaDto.IdGrado,
                p_tipo_matricula = matriculaDto.TipoMatricula,
                p_estado_matricula = matriculaDto.EstadoMatricula,
                p_observaciones = matriculaDto.Observaciones,
                p_usuario_registro = matriculaDto.UsuarioRegistro,
                p_tipo_institucion = matriculaDto.TipoInstitucion
            };

            // La función de base de datos devuelve una sola columna con un resultado en formato JSON.
            // Usamos QuerySingleOrDefaultAsync<string> para leer ese valor.
            try
            {
                const string sql = "SELECT realizar_matricula_colegio(@p_id_alumno, @p_id_periodo, @p_id_grado, @p_tipo_matricula, @p_estado_matricula, @p_observaciones, @p_usuario_registro, @p_tipo_institucion)";

                var jsonResult = await connection.QuerySingleOrDefaultAsync<string>(
                    sql,
                    parameters
                );

                if (string.IsNullOrEmpty(jsonResult))
                {
                    // Si la función no devuelve nada, lanzamos una excepción para que el middleware la capture.
                    throw new InvalidOperationException("La función de matrícula no devolvió un resultado.");
                }

                // Deserializamos a un JsonElement para poder inspeccionar la estructura del JSON devuelto.
                var dbResult = JsonSerializer.Deserialize<JsonElement>(jsonResult);

                // Verificamos si el JSON devuelto contiene la propiedad "success".
                if (dbResult.TryGetProperty("success", out var successElement) && !successElement.GetBoolean())
                {
                    // Si "success" es false, es un error de negocio. Lanzamos la excepción.
                    var message = dbResult.TryGetProperty("message", out var msgElement) ? msgElement.GetString() : "Error desconocido desde la base de datos.";
                    throw new InvalidOperationException(message);
                }

                // Si no hay propiedad "success" o es true, asumimos que es un caso de éxito
                // y que el JSON contiene los datos de la matrícula.
                return new MatriculaResponseDTO
                {
                    Success = true,
                    Message = "Matrícula realizada con éxito.",
                    IdMatricula = dbResult.TryGetProperty("id_matricula", out var idElement) && idElement.ValueKind == JsonValueKind.Number ? idElement.GetInt32() : null,
                    FechaMatricula = dbResult.TryGetProperty("fecha_matricula", out var fechaElement) && fechaElement.ValueKind == JsonValueKind.String ? fechaElement.GetString() : null
                };
            }
            catch (PostgresException ex)
            {
                // Si la función de base de datos lanza una excepción (RAISE EXCEPTION),
                // la capturamos aquí. Asumimos que el mensaje de la excepción es el
                // mensaje de error de negocio que queremos mostrar al usuario.
                return new MatriculaResponseDTO
                {
                    Success = false,
                    Message = ex.MessageText // Usamos directamente el mensaje de la excepción de la BD.
                };
            }
        }

        public async Task<List<MatriculaListarDTO>> ListarMatriculasPorPeriodo(int idPeriodo, string? codigoSede = null)
        {
            await using var connection = await GetConnectionAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@p_id_periodo", idPeriodo, DbType.Int32);
            parameters.Add("@p_codigo_sede", codigoSede);

            var matriculas = await connection.QueryAsync<MatriculaListarDTO>(
                "listar_matriculas_periodo",
                parameters,
                commandType: CommandType.StoredProcedure);

            return matriculas.AsList();
        }

        public async Task<MatriculaDTO?> ObtenerMatriculaPorId(int idMatricula)
        {
            await using var connection = await GetConnectionAsync();

            const string query = @"
                SELECT m.id_matricula, 
                       m.id_alumno,
                       m.fecha_inicio,
                       m.fecha_fin,
                       m.tipo_matricula,
                       m.estado_matricula,
                       m.id_seccion,
                       m.observaciones,
                       m.veces,
                       m.id_periodo,
                       m.id_grado,
                       m.codigo_sede,
                       m.fecha_matricula,
                       m.usuario_registro,
                       m.activo,
                       a.nombre as nombre_alumno, a.apellido_paterno, a.apellido_materno, a.dni as dni_alumno,
                       g.""DESCRIPCION_GRADO"" as descripcion_grado, g.""NIVEL_EDUCATIVO"" as nivel_educativo,
                       p.descripcion_periodo, p.codigo_periodo,
                       s.descripcion_sede
                FROM matricula m
                INNER JOIN alumno a ON m.id_alumno = a.id_alumno
                LEFT JOIN grado g ON m.id_grado = g.""ID_GRADO""
                LEFT JOIN periodoacademico p ON m.id_periodo = p.id_periodo
                LEFT JOIN sede s ON m.codigo_sede = s.codigo_sede
                WHERE m.id_matricula = @IdMatricula AND m.activo = true";

            var matricula = await connection.QueryFirstOrDefaultAsync<MatriculaDTO>(query, new { IdMatricula = idMatricula });

            return matricula;
        }

        public async Task<List<MatriculaDTO>> ObtenerMatriculasPorAlumno(int idAlumno)
        {
            await using var connection = await GetConnectionAsync();
            const string query = @"
                SELECT m.id_matricula, m.id_alumno, m.fecha_inicio, m.fecha_fin, m.tipo_matricula, 
                       m.estado_matricula, m.id_seccion, m.observaciones, m.veces, m.id_periodo, 
                       m.id_grado, m.codigo_sede, m.fecha_matricula, m.usuario_registro, m.activo,
                       a.nombre as nombre_alumno, a.apellido_paterno, a.apellido_materno, a.dni as dni_alumno,
                       g.""DESCRIPCION_GRADO"" as descripcion_grado, g.""NIVEL_EDUCATIVO"" as nivel_educativo,
                       p.descripcion_periodo, p.codigo_periodo,
                       s.descripcion_sede
                FROM matricula m
                INNER JOIN alumno a ON m.id_alumno = a.id_alumno
                LEFT JOIN grado g ON m.id_grado = g.""ID_GRADO""
                LEFT JOIN periodoacademico p ON m.id_periodo = p.id_periodo
                LEFT JOIN sede s ON m.codigo_sede = s.codigo_sede
                WHERE m.id_alumno = @IdAlumno AND m.activo = true
                ORDER BY m.fecha_matricula DESC";

            var matriculas = await connection.QueryAsync<MatriculaDTO>(query, new { IdAlumno = idAlumno });
            return matriculas.AsList();
        }

        public async Task<bool> VerificarMatriculaAlumno(int idAlumno, int idPeriodo)
        {
            await using var connection = await GetConnectionAsync();
            return await connection.ExecuteScalarAsync<bool>(
                "verificar_matricula_alumno",
                new { p_id_alumno = idAlumno, p_id_periodo = idPeriodo },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> ActualizarEstadoMatricula(int idMatricula, string nuevoEstado)
        {
            await using var connection = await GetConnectionAsync();
            const string sql = "SELECT public.actualizar_estado_matricula(@p_id_matricula, @p_nuevo_estado)";
            
            var parameters = new
            {
                p_id_matricula = idMatricula,
                p_nuevo_estado = nuevoEstado
            };

            return await connection.ExecuteScalarAsync<bool>(sql, parameters);
        }

        public async Task<bool> DesactivarMatricula(int idMatricula)
        {
            await using var connection = await GetConnectionAsync();
            const string query = @"
                UPDATE matricula 
                SET activo = false, estado_matricula = 'Inactiva'
                WHERE id_matricula = @IdMatricula";

            var rowsAffected = await connection.ExecuteAsync(query, new { IdMatricula = idMatricula });
            return rowsAffected > 0;
        }

        public async Task<List<MatriculaDTO>> ObtenerMatriculasActivasPorSede()
        {
            await using var connection = await GetConnectionAsync();
            const string query = "SELECT * from listar_matriculas()";

            var matriculas = await connection.QueryAsync<MatriculaDTO>(query);
            return matriculas.AsList();
        }

        public async Task<List<PeriodoAcademicoDTO>> ListarPeriodosDisponiblesParaMatricula(string codigoSede)
        {
            await using var connection = await GetConnectionAsync();

            const string query = @"
                SELECT p.*
                FROM sede s
                JOIN periodoacademico p
                ON (
                    (s.tipo_institucion = 'C' AND p.tipo_periodo = 'Año')
                    OR
                    (s.tipo_institucion = 'I' AND p.tipo_periodo = 'Ciclo')
                )
                WHERE s.codigo_sede = @CodigoSede
                AND (
                    CURRENT_DATE BETWEEN p.fecha_inicio AND p.fecha_fin
                    OR p.fecha_inicio > CURRENT_DATE
                )
                AND p.anio <= (
                    EXTRACT(YEAR FROM CURRENT_DATE)
                    + CASE WHEN EXTRACT(MONTH FROM CURRENT_DATE) >= 10 THEN 1 ELSE 0 END
                )
                ORDER BY p.fecha_inicio ASC;";

            var periodos = await connection.QueryAsync<PeriodoAcademicoDTO>(query, new { CodigoSede = codigoSede });
            return periodos.AsList();
        }

        public async Task<List<CursoSeccionDTO>> GetCursosPorGrado(int idGrado, string tipoInstitucion)
        {
            await using var connection = await GetConnectionAsync();
            const string sql = @"
                SELECT 
                    c.id_curso AS IdCurso,
                    c.codigo_curso AS CodigoCurso,
                    c.descripcion_curso AS DescripcionCurso,
                    COALESCE(
                        JSON_AGG(
                            JSON_BUILD_OBJECT(
                                'id_seccion', s.id_seccion,
                                'codigo_seccion', s.codigo_seccion,
                                'descripcion_seccion', s.descripcion,
                                'horario', JSON_BUILD_OBJECT(
                                    'turno', dsa.turno,
                                    'nombre_dia', h.nombre_dia,
                                    'fecha_inicio', h.fecha_inicio,
                                    'fecha_fin', h.fecha_fin,
                                    'hora_inicio', h.hora_inicio,
                                    'hora_fin', h.hora_fin
                                )
                            )
                        ) FILTER (WHERE s.id_seccion IS NOT NULL),
                        '[]'
                    ) AS SeccionesJson
                FROM grado_curso gc
                INNER JOIN curso c 
                    ON gc.id_curso = c.id_curso
                LEFT JOIN detalleseccionasignada dsa 
                    ON gc.id_curso = dsa.id_curso
                LEFT JOIN seccion s 
                    ON dsa.id_seccion = s.id_seccion
                LEFT JOIN horario h 
                    ON dsa.id_horario = h.id_horario
                WHERE gc.tipo_institucion ILIKE @tipoInstitucion
                  AND gc.""ID_GRADO"" = @idGrado
                GROUP BY c.id_curso, c.codigo_curso, c.descripcion_curso
                ORDER BY c.codigo_curso;
            ";

            var results = await connection.QueryAsync<dynamic>(sql, new { idGrado, tipoInstitucion });

            return results.Select(row => new CursoSeccionDTO
            {
                IdCurso = row.idcurso,
                CodigoCurso = row.codigocurso,
                DescripcionCurso = row.descripcioncurso,
                Secciones = JsonSerializer.Deserialize<List<SeccionInfoDTO>>(row.seccionesjson) ?? new List<SeccionInfoDTO>()
            }).ToList();
        }

        public async Task<List<ReporteNotaDTO>> GetReporteNotas(int idAlumno)
        {
            await using var connection = await GetConnectionAsync();
            const string sql = @"
                WITH notas_filtradas AS (
                    SELECT 
                        mc.id_curso,
                        n.id_subperiodo,
                        n.nota,
                        c.descripcion_curso,
                        pa.id_periodo
                    FROM matricula m
                    INNER JOIN matricula_curso mc 
                        ON m.id_matricula = mc.id_matricula
                    INNER JOIN notas n 
                        ON m.id_periodo = n.id_periodo
                        AND mc.id_curso = n.id_curso
                    INNER JOIN curso c
                        ON mc.id_curso = c.id_curso
                    INNER JOIN periodoacademico pa
					    ON m.id_periodo = pa.id_periodo
                    WHERE 
                        m.id_alumno = @idAlumno
                        AND n.tipo_nota = 'Promedio Final'
                        AND current_date between pa.fecha_inicio and pa.fecha_fin
                )
                SELECT 
                    nf.id_subperiodo,
                    sp.descripcion_subperiodo,
                    nf.descripcion_curso,
                    nf.nota AS promedio_curso,
                    AVG(nf.nota) OVER (PARTITION BY nf.id_subperiodo) AS promedio_bimestre,
                    AVG(nf.nota) OVER () AS promedio_anual
                FROM notas_filtradas nf
                INNER JOIN subperiodos sp
                    ON nf.id_subperiodo = sp.id_subperiodo
                    AND nf.id_periodo = sp.id_periodo
                ORDER BY sp.fecha_inicio, nf.id_curso;";

            var reportes = await connection.QueryAsync<ReporteNotaDTO>(sql, new { idAlumno });
            return reportes.AsList();
        }
    }
}
