using Dapper;
using MyPortalStudent.Domain.DTOs;
using MyPortalStudent.Domain.IServices;
using Npgsql;
using System.Data;
using NpgsqlTypes; // Añadir esta importación
using System.Text.Json;

namespace MyPortalStudent.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IConfiguration _configuration;

        public MatriculaService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<MatriculaResponseDTO> RealizarMatricula(MatriculaRegistrarDTO matriculaDto)
        {
            await using var connection = new NpgsqlConnection(_configuration["ConnectionStrings:DefaultConnection"]!);

            var parameters = new
            {
                p_id_alumno = matriculaDto.IdAlumno,
                p_id_periodo = matriculaDto.IdPeriodo,
                p_id_grado = matriculaDto.IdGrado,
                p_codigo_sede = matriculaDto.CodigoSede,
                p_tipo_matricula = matriculaDto.TipoMatricula,
                p_estado_matricula = matriculaDto.EstadoMatricula,
                p_observaciones = matriculaDto.Observaciones,
                p_usuario_registro = matriculaDto.UsuarioRegistro
            };

            // La función de base de datos devuelve una sola columna con un resultado en formato JSON.
            // Usamos QuerySingleOrDefaultAsync<string> para leer ese valor.
            try
            {
                const string sql = "SELECT realizar_matricula_anual(@p_id_alumno, @p_id_periodo, @p_id_grado, @p_codigo_sede, @p_tipo_matricula, @p_estado_matricula, @p_observaciones, @p_usuario_registro)";
                
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
            await using var connection = new NpgsqlConnection(_configuration["ConnectionStrings:DefaultConnection"]!);

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
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            await using var connection = new NpgsqlConnection(connectionString);

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
            await using var connection = new NpgsqlConnection(_configuration["ConnectionStrings:DefaultConnection"]!);
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
            await using var connection = new NpgsqlConnection(_configuration["ConnectionStrings:DefaultConnection"]!);
            return await connection.ExecuteScalarAsync<bool>(
                "verificar_matricula_alumno",
                new { p_id_alumno = idAlumno, p_id_periodo = idPeriodo },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> ActualizarEstadoMatricula(int idMatricula, string nuevoEstado)
        {
            await using var connection = new NpgsqlConnection(_configuration["ConnectionStrings:DefaultConnection"]!);
            const string query = @"
                UPDATE matricula 
                SET estado_matricula = @NuevoEstado
                WHERE id_matricula = @IdMatricula AND activo = true";

            var rowsAffected = await connection.ExecuteAsync(query, new { NuevoEstado = nuevoEstado, IdMatricula = idMatricula });
            return rowsAffected > 0;
        }

        public async Task<bool> DesactivarMatricula(int idMatricula)
        {
            await using var connection = new NpgsqlConnection(_configuration["ConnectionStrings:DefaultConnection"]!);
            const string query = @"
                UPDATE matricula 
                SET activo = false, estado_matricula = 'Inactiva'
                WHERE id_matricula = @IdMatricula";

            var rowsAffected = await connection.ExecuteAsync(query, new { IdMatricula = idMatricula });
            return rowsAffected > 0;
        }

        public async Task<List<MatriculaDTO>> ObtenerMatriculasActivasPorSede(string codigoSede)
        {
            await using var connection = new NpgsqlConnection(_configuration["ConnectionStrings:DefaultConnection"]!);
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
                WHERE m.codigo_sede = @CodigoSede AND m.activo = true
                ORDER BY m.fecha_matricula DESC";

            var matriculas = await connection.QueryAsync<MatriculaDTO>(query, new { CodigoSede = codigoSede });
            return matriculas.AsList();
        }

        public async Task<List<PeriodoAcademicoDTO>> ListarPeriodosDisponiblesParaMatricula()
        {
            await using var connection = new NpgsqlConnection(_configuration["ConnectionStrings:DefaultConnection"]!);
            
            const string query = @"
                SELECT * FROM periodoacademico
                WHERE (CURRENT_DATE BETWEEN fecha_inicio AND fecha_fin) OR (fecha_inicio > CURRENT_DATE)
                ORDER BY fecha_inicio ASC;";

            var periodos = await connection.QueryAsync<PeriodoAcademicoDTO>(query);
            return periodos.AsList();
        }
    }
}
