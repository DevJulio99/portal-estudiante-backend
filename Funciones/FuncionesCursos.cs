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
                listaAlumnos.Add(new AlumnoDTO
                {
                    id_alumno = (int)reader["id_alumno"],
                    codigoAlumno = reader["codigo_alumno"].ToString() ?? "",
                    nombre = reader["nombre"].ToString() ?? "",
                    apellidoPaterno = reader["apellido_paterno"].ToString() ?? "",
                    apellidoMaterno = reader["apellido_materno"].ToString() ?? "",
                    dni = reader["dni"].ToString() ?? "",
                    correo = reader["correo"].ToString() ?? "",
                    fechaNacimiento = reader["fecha_nacimiento"].ToString() ?? "",
                    telefono = reader["telefono"].ToString() ?? "",
                    direccion = reader["direccion"].ToString() ?? "",
                    fotoPerfil = reader["foto_perfil"].ToString() ?? "",
                    genero = reader["genero"].ToString() ?? "",
                    tipoAlumno = reader["tipo_alumno"].ToString() ?? "",
                    observaciones = reader["observaciones"].ToString() ?? "",
                    apoderado = reader["apoderado"].ToString() ?? "",
                });
            }
            return listaAlumnos;
        }

        public async Task<List<PerfilDTO>> getAlumnosId(int idAlum)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand($"select * from alumno where id_alumno = {idAlum}", connection);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var listaAlumnos = new List<PerfilDTO>([]);

            while (reader.Read())
            {
                listaAlumnos.Add(new PerfilDTO
                {
                    idBanner = "",
                    pidm = "",
                    codLineaNegocio = "",
                    codAlumno = reader["codigo_alumno"].ToString() ?? "",
                    codPersona = "",
                    codUsuario = "",
                    codSede = "",
                    apePatImag = reader["apellido_paterno"].ToString() ?? "",
                    apeMatImag = reader["apellido_materno"].ToString() ?? "",
                    nombresImag = "",
                    tipoDocumento = "",
                    documenIdentida = reader["dni"].ToString() ?? "",
                    codModalidadEstActual = "",
                    codPeriodoActual = "",
                    codPeriodoBanner = "",
                    codPeriodoBannerCatalogo = "",
                    codProductoActual = "",
                    codPrograma = "",
                    codNivel = "",
                    desNivel = "",
                    codCampus = "",
                    desCampus = "",
                    codEstadoAlumno = "",
                    desEstadoAlumno = "",
                    codTipoAlumno = "",
                    desTipoAlumno = reader["tipo_alumno"].ToString() ?? "",
                    codTipoAprendizaje = "",
                    desTipoAprendizaje = "",
                    codTipoIngreso = "",
                    desTipoIngreso = "",
                    desProducto = "",
                    desPrograma = "",
                    usuarioEmail = "",
                    facultad = "",
                    facultadId = "",
                    fotoUrl = "",//reader["foto_perfil"].ToString() ?? "",
                    fechaNacimiento = reader["fecha_nacimiento"].ToString() ?? "",
                    sexo = reader["genero"].ToString() ?? "",
                    telefono = reader["telefono"].ToString() ?? "",
                    celular = "",
                    ciudad = "",
                    direccion = reader["direccion"].ToString() ?? "",
                    egresado = "",
                    ciclo = "",
                    fullName = (reader["nombre"].ToString() ?? "") + " " + (reader["apellido_paterno"].ToString() ?? "") + " " + (reader["apellido_materno"].ToString() ?? ""),
                    tipoPersona = "",
                    codTipoUsuario = "",
                    correoPersonal = reader["correo"].ToString() ?? "",
                    cicloIngreso = "",
                    fotoUrlLow = "",
                    urbanizacion = "",
                    departamento = "",
                    distrito = "",
                    contactoDeEmergenciaNombre = "",
                    contactoDeEmergenciaApellido = "",
                    contactoDeEmergenciaCelular = "",
                    situacionLaboral = "",
                    tipoDeEmpleo = "",
                    modalidadEmpleo = "",
                    empresa = "",
                    ruc = "",
                    direccionEmpresa = "",
                    cargo = "",
                    enlaceLinkedin = "",
                    infoJefeNombre = "",
                    infoJefeCargo = "",
                    infoJefeCorreo = "",
                    infoJefeTelefono = "",
                    zipCode = "",
                    status = "",
                    presentationLetterStatusId = "",
                    phoneHome = "",
                    autorizaAdicionales = false,
                    autorizaAlumni = false,
                    autorizaDatosPersonales = false
                    // id_alumno = (int)reader["id_alumno"],
                    // codigoAlumno = reader["codigo_alumno"].ToString() ?? "",
                    // nombre = reader["nombre"].ToString() ?? "",
                    // apellidoPaterno = reader["apellido_paterno"].ToString() ?? "",
                    // apellidoMaterno = reader["apellido_materno"].ToString() ?? "",
                    // dni = reader["dni"].ToString() ?? "",
                    // correo = reader["correo"].ToString() ?? "",
                    // fechaNacimiento = reader["fecha_nacimiento"].ToString() ?? "",
                    // telefono = reader["telefono"].ToString() ?? "",
                    // direccion = reader["direccion"].ToString() ?? "",
                    // fotoPerfil = reader["foto_perfil"].ToString() ?? "",
                    // genero = reader["genero"].ToString() ?? "",
                    // tipoAlumno = reader["tipo_alumno"].ToString() ?? "",
                    // observaciones = reader["observaciones"].ToString() ?? "",
                    // apoderado = reader["apoderado"].ToString() ?? "",
                });
            }

            return listaAlumnos;
        }

        public async Task<List<HorarioResponse>> getHorarioId(int idAlum, string fechaInicio, string fechaFin)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand($@"select m.id_matricula,m.veces,ds.id_detalle,ds.turno,
                                      ds.rol_docente,h.*,s.codigo_seccion, 
                                      d.nombre as nombre_docente, 
                                      d.apellido_paterno as apellido_paterno_docente, 
                                      d.apellido_materno as apellidos_materno_docente,
                                      d.correo as correo_docente, d.tipo_docente,
                                      c.codigo_curso, c.descripcion_curso, c.modalidad as modalidad_curso,
                                      p.codigo_periodo, p.descripcion_periodo,a.codigo_aula,
                                      a.descripcion_aula, sed.codigo_sede, sed.descripcion_sede
                                      from matricula m
                                      inner join detalleseccionasignada ds
                                      on m.id_seccion = ds.id_seccion
                                      inner join horario h
                                      on ds.id_horario = h.id_horario
                                      inner join seccion s
                                      on ds.id_seccion = s.id_seccion
                                      inner join docente d
                                      on ds.id_docente = d.id_docente
                                      inner join curso c
                                      on ds.id_curso = c.id_curso
                                      inner join periodoacademico p
                                      on ds.id_periodo = p.id_periodo
                                      inner join aula a
                                      on ds.id_aula = a.id_aula
                                      inner join sede sed
                                      on ds.id_sede = sed.id_sede
                                      where m.id_alumno = {idAlum}", connection);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var listaHorarios = new List<HorarioResponse>([]);
            var listaIdsMatriculas = new List<int>([]);


            while (reader.Read())
            {
                int actualIdMatricula = (int)reader["id_matricula"];
                if (!listaIdsMatriculas.Contains(actualIdMatricula))
                {
                    listaIdsMatriculas.Add(actualIdMatricula);
                    listaHorarios.Add(new HorarioResponse
                    {
                        idMatricula = actualIdMatricula,
                        horario = new HorarioDTO
                        {
                            fechaInicio = reader["fecha_inicio"].ToString() ?? "",
                            fechaFin = reader["fecha_fin"].ToString() ?? "",
                            diaNombre = reader["nombre_dia"].ToString() ?? "",
                            diaNumero = reader["numero_dia"].ToString() ?? "",
                            codPeriodo = reader["codigo_periodo"].ToString() ?? ""
                        },
                        detalleHorario = [new DetalleHorarioDTO
                        {
                            titulo = "",
                            nrc = "",
                            descripMetodoEducativo = reader["modalidad_curso"].ToString() ?? "",
                            codmetodoEducativo = "",
                            descripMateria = reader["descripcion_curso"].ToString() ?? "",
                            codMateria = reader["codigo_curso"].ToString() ?? "",
                            codSeccion = reader["codigo_seccion"].ToString() ?? "",
                            descripPartePeriodo = reader["descripcion_periodo"].ToString() ?? "",
                            codPartePeriodo = "",
                            cantidadVeces = reader["veces"].ToString() ?? "",
                            codAula = reader["codigo_aula"].ToString() ?? "",
                            descripAula = reader["descripcion_aula"].ToString() ?? "",
                            codCampus = reader["codigo_sede"].ToString() ?? "",
                            descripCampus = reader["descripcion_aula"].ToString() ?? "",
                            codEdificio = "",
                            descripEdificio = "",
                            fechaInicio = reader["fecha_inicio"].ToString() ?? "",
                            fechaFin = reader["fecha_fin"].ToString() ?? "",
                            diaNombre = reader["nombre_dia"].ToString() ?? "",
                            diaNumero = reader["numero_dia"].ToString() ?? "",
                            horaInicio = reader["hora_inicio"].ToString() ?? "",
                            horaFin = reader["hora_fin"].ToString() ?? "",
                            profesor = [
                                new ProfesorDTO {
                                 idBanner = "",
                                 nombres = reader["nombre_docente"].ToString() ?? "",
                                 apellidos =  (reader["apellido_paterno_docente"].ToString() ?? "") + (reader["apellidos_materno_docente"].ToString() ?? ""),
                                 nombreCompleto = (reader["nombre_docente"].ToString() ?? "") +(reader["apellido_paterno_docente"].ToString() ?? "") + (reader["apellidos_materno_docente"].ToString() ?? ""),
                                 correo = reader["correo_docente"].ToString() ?? "",
                                 pidm = "",
                                 tipoDesc =reader["tipo_docente"].ToString() ?? "",
                                }
                            ],
                            orden = "",
                            codCurso = ""
                        }
                        ]
                    });
                }
                else
                {
                    int indexHorario = listaHorarios.FindIndex(x => x.idMatricula == actualIdMatricula);
                    if (indexHorario >= 0)
                    {
                        listaHorarios[indexHorario].detalleHorario.Append(new DetalleHorarioDTO
                        {
                            titulo = "",
                            nrc = "",
                            descripMetodoEducativo = reader["modalidad_curso"].ToString() ?? "",
                            codmetodoEducativo = "",
                            descripMateria = reader["descripcion_curso"].ToString() ?? "",
                            codMateria = reader["codigo_curso"].ToString() ?? "",
                            codSeccion = reader["codigo_seccion"].ToString() ?? "",
                            descripPartePeriodo = reader["descripcion_periodo"].ToString() ?? "",
                            codPartePeriodo = "",
                            cantidadVeces = reader["veces"].ToString() ?? "",
                            codAula = reader["codigo_aula"].ToString() ?? "",
                            descripAula = reader["descripcion_aula"].ToString() ?? "",
                            codCampus = reader["codigo_sede"].ToString() ?? "",
                            descripCampus = reader["descripcion_aula"].ToString() ?? "",
                            codEdificio = "",
                            descripEdificio = "",
                            fechaInicio = reader["fecha_inicio"].ToString() ?? "",
                            fechaFin = reader["fecha_fin"].ToString() ?? "",
                            diaNombre = reader["nombre_dia"].ToString() ?? "",
                            diaNumero = reader["numero_dia"].ToString() ?? "",
                            horaInicio = reader["hora_inicio"].ToString() ?? "",
                            horaFin = reader["hora_fin"].ToString() ?? "",
                            profesor = [
                                new ProfesorDTO {
                                 idBanner = "",
                                 nombres = reader["nombre_docente"].ToString() ?? "",
                                 apellidos =  (reader["apellido_paterno_docente"].ToString() ?? "") + (reader["apellidos_materno_docente"].ToString() ?? ""),
                                 nombreCompleto = (reader["nombre_docente"].ToString() ?? "") +(reader["apellido_paterno_docente"].ToString() ?? "") + (reader["apellidos_materno_docente"].ToString() ?? ""),
                                 correo = reader["correo_docente"].ToString() ?? "",
                                 pidm = "",
                                 tipoDesc =reader["tipo_docente"].ToString() ?? "",
                                }
                            ],
                            orden = "",
                            codCurso = ""
                        });
                    }

                }


            }
            //apoderado = reader["apoderado"].ToString() ?? "",
            return listaHorarios;
        }

        public async Task<List<CursoDTO>> getCursos(int idAlum)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand($@"select * from matricula m
                                         inner join detalleseccionasignada ds
                                         on m.id_seccion = ds.id_seccion
                                         inner join seccion sec
                                         on m.id_seccion = sec.id_seccion
                                         inner join curso c
                                         on ds.id_curso = c.id_curso
                                         inner join docente doc
                                         on ds.id_docente = doc.id_docente
                                         inner join periodoacademico per
                                         on ds.id_periodo = per.id_periodo
                                         inner join aula a
                                         on ds.id_aula = a.id_aula
                                         where id_alumno = {idAlum}", connection);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var listaCursos = new List<CursoDTO>([]);


            while (reader.Read())
            {
                int idCurso = (int)reader["id_curso"];
                int idHorario = (int)reader["id_horario"];
                var asistencias = await getAsistencias(idAlum, idCurso);
                var inasistencias = asistencias.Where(x => x.estadoAsistencia.ToLower() == "ausente").ToList().Count;
                var listaHorario = await getHorarioCurso(idHorario);
                var tieneHorario = listaHorario.Count > 0;

                listaCursos.Add(new CursoDTO
                {
                    modalidad = reader["modalidad"].ToString() ?? "",
                    codCurso = reader["codigo_curso"].ToString() ?? "",
                    descCurso = reader["descripcion_curso"].ToString() ?? "",
                    periodo = reader["codigo_periodo"].ToString() ?? "",
                    salon = reader["codigo_aula"].ToString() ?? "",
                    seccion = reader["codigo_seccion"].ToString() ?? "",
                    docente = [
                       new DocenteCursoDTO {
                           nombresDocentes = reader["nombre"].ToString() ?? "",
                           apellidoPaternoDocente = "",
                           apellidoMaternoDocente = "",
                           emailDocente = reader["correo"].ToString() ?? "",
                           descCategoriaDocente = reader["tipo_docente"].ToString() ?? "",
                           codCategoriaDocente = "",
                           codUsuarioDocente = ""
                       }
                    ],
                    ciclo = reader["nivel_grado"].ToString() ?? "",
                    creditos = "",
                    cantidadVeces = reader["veces"].ToString() ?? "0",
                    inasistencias = inasistencias > 0 ? inasistencias.ToString() : "0",
                    statusCurso = "Iniciado",
                    orden = 1,
                    notaFinal = 0,
                    tieneHorario = tieneHorario
                });
            }
            return listaCursos;
        }

        public async Task<List<AsistenciaDTO>> getAsistencias(int idAlum, int idCurso)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand($@"select * from asistencias where id_alumno = {idAlum} and id_curso = {idCurso}", connection);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var listaAsistencia = new List<AsistenciaDTO>([]);


            while (reader.Read())
            {
                listaAsistencia.Add(new AsistenciaDTO {
                   idAsistencia = (int)reader["id_asistencia"],
                   dia = reader["dia"].ToString() ?? "",
                   estadoAsistencia = reader["estado_asistencia"].ToString() ?? "",
                   idAlumno = (int)reader["id_alumno"],
                   idCurso = (int)reader["id_curso"],            
                });
            }
            return listaAsistencia;
        }


        public async Task<List<HorarioCursoDTO>> getHorarioCurso(int idHorario)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand($@"select * from horario where id_horario = {idHorario}", connection);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var listaHorario = new List<HorarioCursoDTO>([]);


            while (reader.Read())
            {
                listaHorario.Add(new HorarioCursoDTO {
                   idHorario = (int)reader["id_horario"],     
                   nombreDia = reader["nombre_dia"].ToString() ?? "", 
                   numeroDia = (int)reader["numero_dia"],
                   fechaInicio = reader["fecha_inicio"].ToString() ?? "",
                   fechaFin = reader["fecha_fin"].ToString() ?? "",
                   horaInicio = reader["hora_inicio"].ToString() ?? "",
                   horaFin = reader["hora_fin"].ToString() ?? "",    
                });
            }
            return listaHorario;
        }




    }
}