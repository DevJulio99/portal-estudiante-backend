using MyPortalStudent.Domain;
using MyPortalStudent.Domain.Ifunciones;
using Npgsql;

namespace APIPostulaEnrolamiento.Funciones
{
    public class FuncionesCursos : IFuncionesApi
    {
        private readonly IConfiguration _configuration;

        public FuncionesCursos(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<AlumnoDTO>> getAlumnos()
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.alumno", connection);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var listaAlumnos = new List<AlumnoDTO>([]);
            // double[] data = [];
            
            // var ata_ = reader.Read();

            while (reader.Read())
            {
                // Console.WriteLine(reader["id"]);
                // Use the fetched results
                // var data = reader.GetValue(0);
                  listaAlumnos.Add(new AlumnoDTO {
                    id_alumno = (int)reader["id_alumno"],
                    codigoAlumno = reader["codigo_alumno"].ToString() ?? "",
                    nombre =  reader["nombre"].ToString() ?? "",
                    apellidoPaterno =  reader["apellido_paterno"].ToString() ?? "",
                    apellidoMaterno =  reader["apellido_materno"].ToString() ?? "",
                    dni =  reader["dni"].ToString() ?? "",
                    correo = reader["correo"].ToString() ?? "",
                    fechaNacimiento = reader["fecha_nacimiento"].ToString() ?? "",
                    telefono =  reader["telefono"].ToString() ?? "",
                    direccion =  reader["direccion"].ToString() ?? "",
                    fotoPerfil =  reader["foto_perfil"].ToString() ?? "",
                    genero =  reader["genero"].ToString() ?? "",
                    tipoAlumno =  reader["tipo_alumno"].ToString() ?? "",
                    observaciones =  reader["observaciones"].ToString() ?? "",
                    apoderado =  reader["apoderado"].ToString() ?? "",
                });
            }
            return listaAlumnos;
        }

        public async Task<List<AlumnoDTO>> getAlumnosId(int idAlum)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand($"select * from alumno where id_alumno = {idAlum}", connection);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var listaAlumnos = new List<AlumnoDTO>([]);
            // double[] data = [];
            
            // var ata_ = reader.Read();

            while (reader.Read())
            {
                  listaAlumnos.Add(new AlumnoDTO {
                    id_alumno = (int)reader["id_alumno"],
                    codigoAlumno = reader["codigo_alumno"].ToString() ?? "",
                    nombre =  reader["nombre"].ToString() ?? "",
                    apellidoPaterno =  reader["apellido_paterno"].ToString() ?? "",
                    apellidoMaterno =  reader["apellido_materno"].ToString() ?? "",
                    dni =  reader["dni"].ToString() ?? "",
                    correo = reader["correo"].ToString() ?? "",
                    fechaNacimiento = reader["fecha_nacimiento"].ToString() ?? "",
                    telefono =  reader["telefono"].ToString() ?? "",
                    direccion =  reader["direccion"].ToString() ?? "",
                    fotoPerfil =  reader["foto_perfil"].ToString() ?? "",
                    genero =  reader["genero"].ToString() ?? "",
                    tipoAlumno =  reader["tipo_alumno"].ToString() ?? "",
                    observaciones =  reader["observaciones"].ToString() ?? "",
                    apoderado =  reader["apoderado"].ToString() ?? "",
                });
            }

            return listaAlumnos;
        }
    }
}