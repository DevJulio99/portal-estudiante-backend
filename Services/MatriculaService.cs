using MyPortalStudent.Domain.DTOs;
using MyPortalStudent.Domain.IServices;
using Npgsql;
using System.Data;
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
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand("realizar_matricula_anual", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_id_alumno", matriculaDto.IdAlumno);
                    cmd.Parameters.AddWithValue("@p_id_periodo", matriculaDto.IdPeriodo);
                    cmd.Parameters.AddWithValue("@p_id_grado", matriculaDto.IdGrado);
                    cmd.Parameters.AddWithValue("@p_codigo_sede", matriculaDto.CodigoSede);
                    cmd.Parameters.AddWithValue("@p_tipo_matricula", matriculaDto.TipoMatricula);
                    cmd.Parameters.AddWithValue("@p_estado_matricula", matriculaDto.EstadoMatricula);
                    cmd.Parameters.AddWithValue("@p_observaciones", matriculaDto.Observaciones ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_usuario_registro", matriculaDto.UsuarioRegistro);

                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var jsonResult = reader.GetString(0);
                                var result = JsonSerializer.Deserialize<MatriculaResponseDTO>(jsonResult);
                                return result ?? new MatriculaResponseDTO
                                {
                                    Success = false,
                                    Message = "Error al procesar la respuesta del servidor"
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new MatriculaResponseDTO
                        {
                            Success = false,
                            Message = $"Error al realizar la matrícula: {ex.Message}"
                        };
                    }
                }
            }

            return new MatriculaResponseDTO
            {
                Success = false,
                Message = "Error inesperado al procesar la matrícula"
            };
        }

        public async Task<List<MatriculaListarDTO>> ListarMatriculasPorPeriodo(int idPeriodo, string? codigoSede = null)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            var matriculas = new List<MatriculaListarDTO>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand("listar_matriculas_periodo", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_id_periodo", idPeriodo);
                    cmd.Parameters.AddWithValue("@p_codigo_sede", codigoSede ?? (object)DBNull.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            matriculas.Add(new MatriculaListarDTO
                            {
                                IdMatricula = reader.GetInt32("id_matricula"),
                                NombreAlumno = reader.GetString("nombre_alumno"),
                                ApellidoPaterno = reader.GetString("apellido_paterno"),
                                ApellidoMaterno = reader.GetString("apellido_materno"),
                                Dni = reader.GetString("dni"),
                                DescripcionGrado = reader.GetString("descripcion_grado"),
                                EstadoMatricula = reader.GetString("estado_matricula"),
                                FechaMatricula = reader.GetDateTime("fecha_matricula").ToString("yyyy-MM-dd HH:mm:ss"),
                                CodigoSede = codigoSede ?? "",
                                DescripcionPeriodo = ""
                            });
                        }
                    }
                }
            }

            return matriculas;
        }

        public async Task<MatriculaDTO?> ObtenerMatriculaPorId(int idMatricula)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT m.*, 
                           a.nombre as nombre_alumno, a.apellido_paterno, a.apellido_materno, a.dni as dni_alumno,
                           g.""DESCRIPCION_GRADO"", g.""NIVEL_EDUCATIVO"",
                           p.descripcion_periodo, p.codigo_periodo,
                           s.descripcion_sede
                    FROM matricula m
                    INNER JOIN alumno a ON m.id_alumno = a.id_alumno
                    LEFT JOIN grado g ON m.id_grado = g.""ID_GRADO""
                    LEFT JOIN periodoacademico p ON m.id_periodo = p.id_periodo
                    LEFT JOIN sede s ON m.codigo_sede = s.codigo_sede
                    WHERE m.id_matricula = @idMatricula AND m.activo = true";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@idMatricula", idMatricula);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new MatriculaDTO
                            {
                                IdMatricula = reader.GetInt32("id_matricula"),
                                IdAlumno = reader.GetInt32("id_alumno"),
                                FechaInicio = reader.GetDateTime("fecha_inicio").ToString("yyyy-MM-dd"),
                                FechaFin = reader.GetDateTime("fecha_fin").ToString("yyyy-MM-dd"),
                                TipoMatricula = reader.GetString("tipo_matricula"),
                                EstadoMatricula = reader.GetString("estado_matricula"),
                                IdSeccion = reader.IsDBNull("id_seccion") ? null : reader.GetInt32("id_seccion"),
                                Observaciones = reader.IsDBNull("observaciones") ? null : reader.GetString("observaciones"),
                                Veces = reader.IsDBNull("veces") ? null : reader.GetInt32("veces"),
                                IdPeriodo = reader.GetInt32("id_periodo"),
                                IdGrado = reader.GetInt32("id_grado"),
                                CodigoSede = reader.GetString("codigo_sede"),
                                FechaMatricula = reader.GetDateTime("fecha_matricula").ToString("yyyy-MM-dd HH:mm:ss"),
                                UsuarioRegistro = reader.GetString("usuario_registro"),
                                Activo = reader.GetBoolean("activo"),
                                NombreAlumno = reader.GetString("nombre_alumno"),
                                ApellidoPaterno = reader.GetString("apellido_paterno"),
                                ApellidoMaterno = reader.GetString("apellido_materno"),
                                DniAlumno = reader.GetString("dni_alumno"),
                                DescripcionGrado = reader.IsDBNull("DESCRIPCION_GRADO") ? null : reader.GetString("DESCRIPCION_GRADO"),
                                NivelEducativo = reader.IsDBNull("NIVEL_EDUCATIVO") ? null : reader.GetString("NIVEL_EDUCATIVO"),
                                DescripcionPeriodo = reader.IsDBNull("descripcion_periodo") ? null : reader.GetString("descripcion_periodo"),
                                CodigoPeriodo = reader.IsDBNull("codigo_periodo") ? null : reader.GetString("codigo_periodo"),
                                DescripcionSede = reader.IsDBNull("descripcion_sede") ? null : reader.GetString("descripcion_sede")
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<List<MatriculaDTO>> ObtenerMatriculasPorAlumno(int idAlumno)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            var matriculas = new List<MatriculaDTO>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT m.*, 
                           a.nombre as nombre_alumno, a.apellido_paterno, a.apellido_materno, a.dni as dni_alumno,
                           g.""DESCRIPCION_GRADO"", g.""NIVEL_EDUCATIVO"",
                           p.descripcion_periodo, p.codigo_periodo,
                           s.descripcion_sede
                    FROM matricula m
                    INNER JOIN alumno a ON m.id_alumno = a.id_alumno
                    LEFT JOIN grado g ON m.id_grado = g.""ID_GRADO""
                    LEFT JOIN periodoacademico p ON m.id_periodo = p.id_periodo
                    LEFT JOIN sede s ON m.codigo_sede = s.codigo_sede
                    WHERE m.id_alumno = @idAlumno AND m.activo = true
                    ORDER BY m.fecha_matricula DESC";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@idAlumno", idAlumno);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            matriculas.Add(new MatriculaDTO
                            {
                                IdMatricula = reader.GetInt32("id_matricula"),
                                IdAlumno = reader.GetInt32("id_alumno"),
                                FechaInicio = reader.GetDateTime("fecha_inicio").ToString("yyyy-MM-dd"),
                                FechaFin = reader.GetDateTime("fecha_fin").ToString("yyyy-MM-dd"),
                                TipoMatricula = reader.GetString("tipo_matricula"),
                                EstadoMatricula = reader.GetString("estado_matricula"),
                                IdSeccion = reader.IsDBNull("id_seccion") ? null : reader.GetInt32("id_seccion"),
                                Observaciones = reader.IsDBNull("observaciones") ? null : reader.GetString("observaciones"),
                                Veces = reader.IsDBNull("veces") ? null : reader.GetInt32("veces"),
                                IdPeriodo = reader.GetInt32("id_periodo"),
                                IdGrado = reader.GetInt32("id_grado"),
                                CodigoSede = reader.GetString("codigo_sede"),
                                FechaMatricula = reader.GetDateTime("fecha_matricula").ToString("yyyy-MM-dd HH:mm:ss"),
                                UsuarioRegistro = reader.GetString("usuario_registro"),
                                Activo = reader.GetBoolean("activo"),
                                NombreAlumno = reader.GetString("nombre_alumno"),
                                ApellidoPaterno = reader.GetString("apellido_paterno"),
                                ApellidoMaterno = reader.GetString("apellido_materno"),
                                DniAlumno = reader.GetString("dni_alumno"),
                                DescripcionGrado = reader.IsDBNull("DESCRIPCION_GRADO") ? null : reader.GetString("DESCRIPCION_GRADO"),
                                NivelEducativo = reader.IsDBNull("NIVEL_EDUCATIVO") ? null : reader.GetString("NIVEL_EDUCATIVO"),
                                DescripcionPeriodo = reader.IsDBNull("descripcion_periodo") ? null : reader.GetString("descripcion_periodo"),
                                CodigoPeriodo = reader.IsDBNull("codigo_periodo") ? null : reader.GetString("codigo_periodo"),
                                DescripcionSede = reader.IsDBNull("descripcion_sede") ? null : reader.GetString("descripcion_sede")
                            });
                        }
                    }
                }
            }

            return matriculas;
        }

        public async Task<bool> VerificarMatriculaAlumno(int idAlumno, int idPeriodo)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand("verificar_matricula_alumno", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_id_alumno", idAlumno);
                    cmd.Parameters.AddWithValue("@p_id_periodo", idPeriodo);

                    var result = cmd.ExecuteScalar();
                    return Convert.ToBoolean(result);
                }
            }
        }

        public async Task<bool> ActualizarEstadoMatricula(int idMatricula, string nuevoEstado)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    UPDATE matricula 
                    SET estado_matricula = @nuevoEstado
                    WHERE id_matricula = @idMatricula AND activo = true";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@nuevoEstado", nuevoEstado);
                    cmd.Parameters.AddWithValue("@idMatricula", idMatricula);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> DesactivarMatricula(int idMatricula)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    UPDATE matricula 
                    SET activo = false, estado_matricula = 'Inactiva'
                    WHERE id_matricula = @idMatricula";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@idMatricula", idMatricula);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<List<MatriculaDTO>> ObtenerMatriculasActivasPorSede(string codigoSede)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            var matriculas = new List<MatriculaDTO>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT m.*, 
                           a.nombre as nombre_alumno, a.apellido_paterno, a.apellido_materno, a.dni as dni_alumno,
                           g.""DESCRIPCION_GRADO"", g.""NIVEL_EDUCATIVO"",
                           p.descripcion_periodo, p.codigo_periodo,
                           s.descripcion_sede
                    FROM matricula m
                    INNER JOIN alumno a ON m.id_alumno = a.id_alumno
                    LEFT JOIN grado g ON m.id_grado = g.""ID_GRADO""
                    LEFT JOIN periodoacademico p ON m.id_periodo = p.id_periodo
                    LEFT JOIN sede s ON m.codigo_sede = s.codigo_sede
                    WHERE m.codigo_sede = @codigoSede AND m.activo = true
                    ORDER BY m.fecha_matricula DESC";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@codigoSede", codigoSede);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            matriculas.Add(new MatriculaDTO
                            {
                                IdMatricula = reader.GetInt32("id_matricula"),
                                IdAlumno = reader.GetInt32("id_alumno"),
                                FechaInicio = reader.GetDateTime("fecha_inicio").ToString("yyyy-MM-dd"),
                                FechaFin = reader.GetDateTime("fecha_fin").ToString("yyyy-MM-dd"),
                                TipoMatricula = reader.GetString("tipo_matricula"),
                                EstadoMatricula = reader.GetString("estado_matricula"),
                                IdSeccion = reader.IsDBNull("id_seccion") ? null : reader.GetInt32("id_seccion"),
                                Observaciones = reader.IsDBNull("observaciones") ? null : reader.GetString("observaciones"),
                                Veces = reader.IsDBNull("veces") ? null : reader.GetInt32("veces"),
                                IdPeriodo = reader.GetInt32("id_periodo"),
                                IdGrado = reader.GetInt32("id_grado"),
                                CodigoSede = reader.GetString("codigo_sede"),
                                FechaMatricula = reader.GetDateTime("fecha_matricula").ToString("yyyy-MM-dd HH:mm:ss"),
                                UsuarioRegistro = reader.GetString("usuario_registro"),
                                Activo = reader.GetBoolean("activo"),
                                NombreAlumno = reader.GetString("nombre_alumno"),
                                ApellidoPaterno = reader.GetString("apellido_paterno"),
                                ApellidoMaterno = reader.GetString("apellido_materno"),
                                DniAlumno = reader.GetString("dni_alumno"),
                                DescripcionGrado = reader.IsDBNull("DESCRIPCION_GRADO") ? null : reader.GetString("DESCRIPCION_GRADO"),
                                NivelEducativo = reader.IsDBNull("NIVEL_EDUCATIVO") ? null : reader.GetString("NIVEL_EDUCATIVO"),
                                DescripcionPeriodo = reader.IsDBNull("descripcion_periodo") ? null : reader.GetString("descripcion_periodo"),
                                CodigoPeriodo = reader.IsDBNull("codigo_periodo") ? null : reader.GetString("codigo_periodo"),
                                DescripcionSede = reader.IsDBNull("descripcion_sede") ? null : reader.GetString("descripcion_sede")
                            });
                        }
                    }
                }
            }

            return matriculas;
        }
    }
}
