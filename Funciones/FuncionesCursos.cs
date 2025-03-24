using System.Globalization;
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
                    idGrado = Int32.TryParse(reader["id_grado_alumno"].ToString(), out var idGrado) ? idGrado : 0,
                    habilitadoPrueba = Boolean.TryParse(reader["habilitado_prueba_alumno"].ToString(), out var habilitadoPrueba) && habilitadoPrueba
                });
            }
            return listaAlumnos;
        }

        public async Task<Boolean> existeAlumno(string? numDocUsuario)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            Boolean existe = false;

            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand(@"select 1 from alumno
                   where dni = @dni", connection))
            {
                cmd.Parameters.AddWithValue("@dni", numDocUsuario);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existe = true;
                    }
                }
            }

            return existe;
        }

        public async Task<string> asistenciasPorCursoAlumno(AsistenciaCursoAlumnoDTO asistenciaCursoAlumnoDto)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            string total = "0";

            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand(@"select * from asistencias_por_curso_alumno(
                  @idAlumno,@anio, @inicioPeriodo,@finalPeriodo, @codCurso, @estadoAsistencia)", connection))
            {
                cmd.Parameters.AddWithValue("@idAlumno", asistenciaCursoAlumnoDto.idAlumno);
                cmd.Parameters.AddWithValue("@anio", asistenciaCursoAlumnoDto.anio);
                cmd.Parameters.AddWithValue("@inicioPeriodo", asistenciaCursoAlumnoDto.inicioPeriodo);
                cmd.Parameters.AddWithValue("@finalPeriodo", asistenciaCursoAlumnoDto.finalPeriodo);
                cmd.Parameters.AddWithValue("@codCurso", asistenciaCursoAlumnoDto.codigoCurso);
                cmd.Parameters.AddWithValue("@estadoAsistencia", asistenciaCursoAlumnoDto.estadoAsistencia);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        total = reader["cantidad"].ToString() ?? "0";
                    }
                }
            }

            return total;
        }

        public async Task<List<PerfilDTO>> getAlumnosId(string? numDocUsuario)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            var listaAlumnos = new List<PerfilDTO>([]);
            var existeAlumno_ = await existeAlumno(numDocUsuario);

            if(existeAlumno_){
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand($"select * from alumno where dni = @numDocUsuario", connection);
            cmd.Parameters.AddWithValue("@numDocUsuario", numDocUsuario);

            using NpgsqlDataReader reader = cmd.ExecuteReader();

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
                });
            }
            }else {
                listaAlumnos.Add(new PerfilDTO
                {
                    idBanner = "",
                    pidm = "",
                    codLineaNegocio = "",
                    codAlumno = "",
                    codPersona = "",
                    codUsuario = "",
                    codSede = "",
                    apePatImag = "",
                    apeMatImag = "",
                    nombresImag = "",
                    tipoDocumento = "",
                    documenIdentida = "",
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
                    desTipoAlumno = "",
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
                    fechaNacimiento = "",
                    sexo = "",
                    telefono = "",
                    celular = "",
                    ciudad = "",
                    direccion = "",
                    egresado = "",
                    ciclo = "",
                    fullName = "otro usuarip",
                    tipoPersona = "",
                    codTipoUsuario = "",
                    correoPersonal = "",
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
                //if (!listaIdsMatriculas.Contains(actualIdMatricula))
                //{
                    //listaIdsMatriculas.Add(actualIdMatricula);
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
                                 apellidos =  (reader["apellido_paterno_docente"].ToString() ?? "") + " " + (reader["apellidos_materno_docente"].ToString() ?? ""),
                                 nombreCompleto = (reader["nombre_docente"].ToString() ?? "") + " " + (reader["apellido_paterno_docente"].ToString() ?? "") + " " + (reader["apellidos_materno_docente"].ToString() ?? ""),
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
                //}
                // else
                // {
                //     int indexHorario = listaHorarios.FindIndex(x => x.idMatricula == actualIdMatricula);
                //     if (indexHorario >= 0)
                //     {
                //         listaHorarios[indexHorario].detalleHorario.Append(new DetalleHorarioDTO
                //         {
                //             titulo = "",
                //             nrc = "",
                //             descripMetodoEducativo = reader["modalidad_curso"].ToString() ?? "",
                //             codmetodoEducativo = "",
                //             descripMateria = reader["descripcion_curso"].ToString() ?? "",
                //             codMateria = reader["codigo_curso"].ToString() ?? "",
                //             codSeccion = reader["codigo_seccion"].ToString() ?? "",
                //             descripPartePeriodo = reader["descripcion_periodo"].ToString() ?? "",
                //             codPartePeriodo = "",
                //             cantidadVeces = reader["veces"].ToString() ?? "",
                //             codAula = reader["codigo_aula"].ToString() ?? "",
                //             descripAula = reader["descripcion_aula"].ToString() ?? "",
                //             codCampus = reader["codigo_sede"].ToString() ?? "",
                //             descripCampus = reader["descripcion_aula"].ToString() ?? "",
                //             codEdificio = "",
                //             descripEdificio = "",
                //             fechaInicio = reader["fecha_inicio"].ToString() ?? "",
                //             fechaFin = reader["fecha_fin"].ToString() ?? "",
                //             diaNombre = reader["nombre_dia"].ToString() ?? "",
                //             diaNumero = reader["numero_dia"].ToString() ?? "",
                //             horaInicio = reader["hora_inicio"].ToString() ?? "",
                //             horaFin = reader["hora_fin"].ToString() ?? "",
                //             profesor = [
                //                 new ProfesorDTO {
                //                  idBanner = "",
                //                  nombres = reader["nombre_docente"].ToString() ?? "",
                //                  apellidos =  (reader["apellido_paterno_docente"].ToString() ?? "") + " " + (reader["apellidos_materno_docente"].ToString() ?? ""),
                //                  nombreCompleto = (reader["nombre_docente"].ToString() ?? "") + " " + (reader["apellido_paterno_docente"].ToString() ?? "") + " " + (reader["apellidos_materno_docente"].ToString() ?? ""),
                //                  correo = reader["correo_docente"].ToString() ?? "",
                //                  pidm = "",
                //                  tipoDesc =reader["tipo_docente"].ToString() ?? "",
                //                 }
                //             ],
                //             orden = "",
                //             codCurso = ""
                //         });
                //     }

                // }


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
                    ciclo = reader["ciclo"].ToString() ?? "",
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

        public async Task<List<ReporteMatriculaColegioDTO>> getCursosColegio(int idAlum, int anio)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * from public.obtener_reporte_matricula_colegio({idAlum},{anio})", connection);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var listaReporteColegio = new List<ReporteMatriculaColegioDTO>([]);


            while (reader.Read())
            {
                var fechaActual = DateTime.Now;
                var fechaActualTiempo = fechaActual.Ticks;
                var fechaInicioString = reader["fecha_inicio"].ToString();
                var fechaFinString = reader["fecha_fin"].ToString();
                var fechaIniciotiempo = DateTime.ParseExact(fechaInicioString, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                var fechaFintiempo = DateTime.ParseExact(fechaFinString, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                var actualBimestre = fechaActualTiempo >= fechaIniciotiempo.Ticks && fechaActualTiempo <= fechaFintiempo.Ticks;
                var codCurso = reader["cod_cursos_matriculados"].ToString() ?? "";
                var request = new AsistenciaCursoAlumnoDTO(){
                    idAlumno = idAlum,
                    anio = anio,
                    inicioPeriodo = DateTime.Parse(fechaInicioString),
                    finalPeriodo = DateTime.Parse(fechaFinString),
                    codigoCurso = codCurso,
                    estadoAsistencia = "Ausente"
                };
                var inasistencias = await asistenciasPorCursoAlumno(request);

                if(actualBimestre){
                   listaReporteColegio.Add(new ReporteMatriculaColegioDTO {
                    modalidad = "Presencial",
                    codCurso = codCurso,
                    descCurso = reader["cursos_matriculados"].ToString() ?? "",
                    periodo = reader["periodo_academico"].ToString() ?? "",
                    salon = "",
                    seccion = reader["seccion"].ToString() ?? "",
                    docente = [
                       new DocenteCursoDTO {
                           nombresDocentes = reader["docente_nombre"].ToString() ?? "",
                           apellidoPaternoDocente = "",
                           apellidoMaternoDocente = "",
                           emailDocente = reader["docente_email"].ToString() ?? "",
                           descCategoriaDocente = "",
                           codCategoriaDocente = "",
                           codUsuarioDocente = ""
                       }
                    ],
                    ciclo = "",
                    creditos = "",
                    cantidadVeces = "0",
                    inasistencias = inasistencias,
                    statusCurso = "Iniciado",
                    orden = 1,
                    notaFinal = 0,
                    tieneHorario = false,
                    grado = reader["grado"].ToString() ?? "",
                    nivel = reader["nivel"].ToString() ?? "",
                    periodoAcademico = reader["periodo_academico"].ToString() ?? "",
                    fechaInicio = reader["fecha_inicio"].ToString() ?? "",
                    fechaFin = reader["fecha_fin"].ToString() ?? "",       
                });
                }
                
            }
            return listaReporteColegio;
        }


        public async Task<List<AlumnoAsistenciaDTO>> getAsistenciasAlumno(int idAlum, string bimester, string codCurso , int anio)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * from obtener_asistencias_alumno_bimestre({idAlum},'{bimester}','{codCurso}',{anio})", connection);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var listaAsistencias = new List<AlumnoAsistenciaDTO>([]);


            while (reader.Read())
            {
                   listaAsistencias.Add(new AlumnoAsistenciaDTO {
                    idAsistencia = (int)reader["id_asistencia"],
                    dia = reader["dia"].ToString() ?? "",  
                    estadoAsistencia = reader["estado_asistencia"].ToString() ?? "",    
                    descripcionCurso = reader["descripcion_curso"].ToString() ?? "",
                    modalidad = reader["modalidad"].ToString() ?? "",
                    horaInicio =  reader["hora_inicio"].ToString() ?? "",
                    horaFin = reader["hora_fin"].ToString() ?? "",
                });
                
            }
            return listaAsistencias;
        }

        public async Task<List<HorarioxAulaDTO>> getHorariosxAula(int idAula)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * from obtener_horarios_aula({idAula})", connection);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var listaHorarios = new List<HorarioxAulaDTO>([]);


            while (reader.Read())
            {
                   listaHorarios.Add(new HorarioxAulaDTO {
                    descripcionCurso = reader["descripcion_curso"].ToString() ?? "", 
                    descripcionAula = reader["descripcion_aula"].ToString() ?? "",
                    nombreDia = reader["nombre_dia"].ToString() ?? "",
                    horaInicio = reader["hora_inicio"].ToString() ?? "",
                    horaFin = reader["hora_fin"].ToString() ?? "",
                    nombreDocente = reader["nombre_docente"].ToString() ?? "",
                    apellidoPaternoDocente = reader["apellido_paterno_docente"].ToString() ?? "",
                    apellidoMaternoDocente = reader["apellido_materno_docente"].ToString() ?? "",
                    seccion = reader["seccion"].ToString() ?? "",        
                });
                
            }
            return listaHorarios;
        }

        public async Task<List<HorarioCursoxAlumnnoDTO>> getHorariosCursoxAlumno(int idAlumno)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * from obtener_horarios_cursos_por_alumno({idAlumno})", connection);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var listaHorarios = new List<HorarioCursoxAlumnnoDTO>([]);


            while (reader.Read())
            {
                   listaHorarios.Add(new HorarioCursoxAlumnnoDTO {
                    nombreAlumno = reader["nombre_alumno"].ToString() ?? "",
                    apellidoPaterno = reader["apellido_paterno"].ToString() ?? "",
                    apellidoMaterno = reader["apellido_materno"].ToString() ?? "",
                    descripcionSeccion = reader["descripcion_seccion"].ToString() ?? "",    
                    descripcionCurso = reader["descripcion_curso"].ToString() ?? "", 
                    nombreDia = reader["nombre_dia"].ToString() ?? "",
                    horaInicio = reader["hora_inicio"].ToString() ?? "",
                    horaFin = reader["hora_fin"].ToString() ?? "",       
                });
                
            }
            return listaHorarios;
        }

        public async Task<List<HorarioCursoxDocenteDTO>> getHorarioCursoxDocente(int idDocente)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * from obtener_horarios_cursos_por_docente({idDocente})", connection);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var listaHorarios = new List<HorarioCursoxDocenteDTO>([]);


            while (reader.Read())
            {
                   listaHorarios.Add(new HorarioCursoxDocenteDTO {
                    nombreDocente = reader["nombre_docente"].ToString() ?? "",
                    apellidoPaterno = reader["apellido_paterno"].ToString() ?? "",
                    apellidoMaterno = reader["apellido_materno"].ToString() ?? "",
                    descripcionSeccion = reader["descripcion_seccion"].ToString() ?? "",    
                    descripcionCurso = reader["descripcion_curso"].ToString() ?? "", 
                    nombreDia = reader["nombre_dia"].ToString() ?? "",
                    horaInicio = reader["hora_inicio"].ToString() ?? "",
                    horaFin = reader["hora_fin"].ToString() ?? "",       
                });
                
            }
            return listaHorarios;
        }

        public async Task<List<NotasxBimestreDTO>> getNotasxBimestre(int idAlum, string tipoPeriodo, int anio)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * from obtener_notas_por_bimestre({idAlum}, '{tipoPeriodo}', {anio})", connection);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var listaNotas = new List<NotasxBimestreDTO>([]);


            while (reader.Read())
            {
                   listaNotas.Add(new NotasxBimestreDTO {
                    alumno = reader["alumno"].ToString() ?? "",
                    apellidoPaterno = reader["apellido_paterno"].ToString() ?? "",
                    apellidoMaterno = reader["apellido_materno"].ToString() ?? "",   
                    descripcionCurso = reader["descripcion_curso"].ToString() ?? "",    
                    codigoPeriodo = reader["codigo_periodo"].ToString() ?? "",
                    descripcionPeriodo = reader["descripcion_periodo"].ToString() ?? "",   
                    nota = reader["nota"].ToString() ?? "",
                    peso = reader["peso"].ToString() ?? "",
                    tipoNota = reader["tipo_nota"].ToString() ?? "",
                });
                
            }
            return listaNotas;
        }

        public async Task<List<PagoDTO>> getPagosPorAlumno(int idAlumno, int anio)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM get_pagos_por_alumno(@id_alumno, @anio)", connection);
            cmd.Parameters.AddWithValue("id_alumno", idAlumno);
            cmd.Parameters.AddWithValue("anio", anio);


            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            
            var pagosList = new List<PagoDTO>();

            while (await reader.ReadAsync())
            {
                pagosList.Add(new PagoDTO
                {
                    IdPago = (int)reader["id_pago"],
                    DocumentoPago = reader["documento_pago"].ToString() ?? "",
                    FechaVencimiento = (DateTime)reader["f_vencimiento"],
                    Ciclo = reader["ciclo"].ToString() ?? "",
                    Saldo = (decimal)reader["saldo"],
                    Mora = (decimal)reader["mora"],
                    TotalAPagar = (decimal)reader["total_a_pagar"],
                    Detalle = reader["detalle"].ToString() ?? "",
                    Imagen = reader["imagen"].ToString() ?? "",
                    Anio = (int)reader["anio"]
                });
            }

            return pagosList;
        }

         public async Task<List<PagoDTO>> getPagosPorSede(SedePaginadoDTO sedePaginadoDto)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
             int pagina = 0;

            if(sedePaginadoDto.pagina > 1){
                pagina = (sedePaginadoDto.pagina - 1) * sedePaginadoDto.itemsPorPagina;
            }

            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand($@"select * from listar_pagos_por_sede_paginado(@codSede, @pagina, @itemPagina)", connection);
            cmd.Parameters.AddWithValue("codSede", sedePaginadoDto.codigoSede);
            cmd.Parameters.AddWithValue("pagina", pagina);
            cmd.Parameters.AddWithValue("itemPagina", sedePaginadoDto.itemsPorPagina);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            
            var pagosList = new List<PagoDTO>();

            while (await reader.ReadAsync())
            {
                pagosList.Add(new PagoDTO
                {
                    IdPago = (int)reader["idpago"],
                    DocumentoPago = reader["documentopago"].ToString() ?? "",
                    FechaVencimiento = (DateTime)reader["fechav"],
                    Ciclo = reader["ciclopago"].ToString() ?? "",
                    Saldo = (decimal)reader["saldopago"],
                    Mora = (decimal)reader["morapago"],
                    TotalAPagar = (decimal)reader["totalpago"],
                    Detalle = reader["detallepago"].ToString() ?? "",
                    Imagen = reader["imagepago"].ToString() ?? "",
                    Anio = (int)reader["aniopago"],
                    total = Int32.Parse(reader["total_resultados"].ToString() ?? "0")
                });
            }

            return pagosList;
        }

        public async Task<List<CalendarioAcademicoDTO>> GetCalendarioAcademico(int anio)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Consulta SQL para obtener los calendarios para el año solicitado
            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM calendario_academico WHERE EXTRACT(YEAR FROM fecha_inicio) = @anio OR EXTRACT(YEAR FROM fecha_fin) = @anio", connection);
            cmd.Parameters.AddWithValue("anio", anio);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            
            var calendarioList = new List<CalendarioAcademicoDTO>();

            while (await reader.ReadAsync())
            {
                calendarioList.Add(new CalendarioAcademicoDTO
                {
                    IdCalendario = (int)reader["id_calendario"],
                    Actividad = reader["actividad"].ToString() ?? "",
                    FechaInicio = ((DateTime)reader["fecha_inicio"]).ToString("yyyy-MM-dd"),
                    FechaFin = ((DateTime)reader["fecha_fin"]).ToString("yyyy-MM-dd"),
                    ModalidadEstudios = reader["modalidad_estudios"].ToString() ?? "",
                    TipoActividad = reader["tipo_actividad"].ToString() ?? ""
                });
            }

            return calendarioList;
        }

        public async Task<List<CategoriaDocumentoDTO>> GetDocumentosConCategoria()
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            string query = @"
                SELECT 
                    c.""ID_CATEGORIA_DOCUMENTO"",
                    c.""STATUS"" AS categoria_status,
                    c.""NOMBRE"" AS categoria_nombre,
                    c.""DESCRIPCION"" AS categoria_descripcion,
                    c.""IMAGEN"" AS categoria_imagen,
                    c.""SECUENCIA"" AS categoria_secuencia,
                    c.""DATE_CREATED"" AS categoria_date_created,
                    d.""ID_DOCUMENTO"",
                    d.""STATUS"" AS documento_status,
                    d.""TITULO"" AS documento_titulo,
                    d.""DESCRIPCION"" AS documento_descripcion,
                    d.""ENLACE"" AS documento_enlace,
                    d.""SECUENCIA"" AS documento_secuencia,
                    d.""DATE_CREATED"" AS documento_date_created,
                    d.""TIPO_DOCUMENTO"",
                    d.""MAS_BUSCADOS"",
                    d.""SECUENCIA_MAS_BUSCADA"",
                    d.""DOCUMENTO_VER"",
                    d.""INTERNO"",
                    d.""FECHA_ACTUALIZACION"",
                    d.""FECHA_INICIO"",
                    d.""FECHA_FIN"",
                    d.""DOCUMENTO_DESCARGA"",
                    d.""NOMBRE_DOCUMENTO"",
                    d.""TYPE"" AS documento_type
                FROM categoria_documento c
                LEFT JOIN documentos d ON c.""ID_CATEGORIA_DOCUMENTO"" = d.""ID_CATEGORIA_DOCUMENTO""
                ORDER BY c.""SECUENCIA"", d.""SECUENCIA"";
            ";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, connection);
            
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            
            var categoriaList = new List<CategoriaDocumentoDTO>();

            while (await reader.ReadAsync())
            {
                var categoriaId = (int)reader["ID_CATEGORIA_DOCUMENTO"];
                var categoria = categoriaList.FirstOrDefault(c => c.Id == categoriaId);

                if (categoria == null)
                {
                    categoria = new CategoriaDocumentoDTO
                    {
                        Id = categoriaId,
                        Status = reader["categoria_status"].ToString() ?? "",
                        Nombre = reader["categoria_nombre"].ToString() ?? "",
                        Descripcion = reader["categoria_descripcion"].ToString(),
                        Imagen = reader["categoria_imagen"].ToString(),
                        Secuencia = (int)reader["categoria_secuencia"],
                        DateCreated = (DateTime)reader["categoria_date_created"],
                        Documentos = new List<DocumentoDTO>()
                    };
                    categoriaList.Add(categoria);
                }

                categoria.Documentos.Add(new DocumentoDTO
                {
                    Id = (int)reader["ID_DOCUMENTO"],
                    Status = reader["documento_status"].ToString() ?? "",
                    Titulo = reader["documento_titulo"].ToString() ?? "",
                    Descripcion = reader["documento_descripcion"].ToString(),
                    Enlace = reader["documento_enlace"].ToString(),
                    Secuencia = (int)reader["documento_secuencia"],
                    DateCreated = (DateTime)reader["documento_date_created"],
                    TipoDocumento = reader["TIPO_DOCUMENTO"].ToString(),
                    MasBuscados = (bool)reader["MAS_BUSCADOS"],
                    SecuenciaMasBuscada = reader["SECUENCIA_MAS_BUSCADA"] as int?,
                    Documento = reader["DOCUMENTO_VER"].ToString(),
                    Interno = (bool)reader["INTERNO"],
                    FechaActualizacion = reader["FECHA_ACTUALIZACION"] as DateTime?,
                    FechaInicio = (DateTime)reader["FECHA_INICIO"],
                    FechaFin = (DateTime)reader["FECHA_FIN"],
                    DocumentoDescarga = reader["DOCUMENTO_DESCARGA"].ToString(),
                    NombreDocumento = reader["NOMBRE_DOCUMENTO"].ToString(),
                    Type = reader["documento_type"].ToString()
                });
            }

            return categoriaList;
        }


        public async Task<List<EventoDTO>> GetEventos()
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            using NpgsqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string query = "SELECT * FROM eventos";

            using NpgsqlCommand cmd = new(query, connection);
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            List<EventoDTO> eventosList = new();

            while (await reader.ReadAsync())
            {
                eventosList.Add(new EventoDTO
                {
                    Titulo = reader["TITULO"].ToString() ?? "",
                    ImagenDesktop = reader["IMAGEN_DESKTOP"].ToString() ?? "",
                    ImagenMobile = reader["IMAGEN_MOBILE"].ToString() ?? "",
                    AltImagenDesktop = reader["ALT_IMAGEN_DESKTOP"] as string,
                    AltImagenMobile = reader["ALT_IMAGEN_MOBILE"] as string,
                    Url = reader["URL"] as string,
                    Prioridad = reader["PRIORIDAD"].ToString() ?? "1",
                    AbrirNuevaPagina = reader["ABRIR_NUEVA_PAGINA"] != DBNull.Value && (bool)reader["ABRIR_NUEVA_PAGINA"],
                    TipoDeEvento = reader["TIPO_DE_EVENTO"].ToString() ?? "",
                    CategoriaEvento = reader["CATEGORIA_EVENTO"].ToString() ?? "",
                    FechaDeInicio = reader["FECHA_DE_INICIO"] != DBNull.Value ? ((DateTime)reader["FECHA_DE_INICIO"]).ToString("yyyy-MM-dd") : "",
                    HoraDeInicio = reader["HORA_DE_INICIO"] != DBNull.Value ? ((TimeSpan)reader["HORA_DE_INICIO"]).ToString(@"hh\:mm\:ss") : "",
                    FechaDeFin = reader["FECHA_DE_FIN"] != DBNull.Value ? ((DateTime)reader["FECHA_DE_FIN"]).ToString("yyyy-MM-dd") : "",
                    HoraDeFin = reader["HORA_DE_FIN"] != DBNull.Value ? ((TimeSpan)reader["HORA_DE_FIN"]).ToString(@"hh\:mm\:ss") : "",
                    FechaInicioEvento = reader["FECHA_INICIO_EVENTO"] != DBNull.Value ? ((DateTime)reader["FECHA_INICIO_EVENTO"]).ToString("yyyy-MM-dd") : "",
                    HoraInicioEvento = reader["HORA_INICIO_EVENTO"] != DBNull.Value ? ((TimeSpan)reader["HORA_INICIO_EVENTO"]).ToString(@"hh\:mm\:ss") : "",
                    FechaFinEvento = reader["FECHA_FIN_EVENTO"] != DBNull.Value ? ((DateTime)reader["FECHA_FIN_EVENTO"]).ToString("yyyy-MM-dd") : "",
                    NombreBoton = reader["NOMBRE_BOTON"] as string,
                    Descripcion = reader["DESCRIPCION"].ToString() ?? "",
                    Id = (int)reader["ID_EVENTO"],
                    Capacidad = reader["CAPACIDAD"].ToString() ?? "0",
                    Ubicacion = await GetUbicacionesEvento((int)reader["ID_EVENTO"])
                });
            }

            return eventosList;
        }

        public async Task<List<UbicacionEventoDTO>> GetUbicacionesEvento(int eventoId)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            using NpgsqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string query = "SELECT * FROM ubicaciones_evento WHERE \"EVENTO_ID\" = @eventoId";

            using NpgsqlCommand cmd = new(query, connection);
            cmd.Parameters.AddWithValue("eventoId", eventoId);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            List<UbicacionEventoDTO> ubicaciones = new();

            while (await reader.ReadAsync())
            {
                ubicaciones.Add(new UbicacionEventoDTO
                {
                    Latitud = reader["LATITUD"].ToString() ?? "",
                    Longitud = reader["LONGITUD"].ToString() ?? "",
                    Direccion = reader["DIRECCION"].ToString() ?? "",
                    Nombre = reader["NOMBRE"].ToString() ?? "",
                    Url = reader["URL"] as string,
                    UrlMobile = reader["URL_MOBILE"] as string
                });
            }

            return ubicaciones;
        }

        public async Task<List<ObligacionPorPeriodoDTO>> GetObligacionesPagadas(int idAlumno)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            
            using NpgsqlConnection connection = new(connectionString);
            await connection.OpenAsync();
            
            string query = "SELECT * FROM obtener_obligaciones_pagadas_por_alumno(@idAlumnoParam)";
            
            using NpgsqlCommand cmd = new(query, connection);
            cmd.Parameters.AddWithValue("idAlumnoParam", idAlumno);
            
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            
            var obligacionesPagadas = new List<ObligacionPagadaDTO>();
            
            while (await reader.ReadAsync())
            {
                obligacionesPagadas.Add(new ObligacionPagadaDTO
                {
                    Periodo = reader["periodo"].ToString() ?? "",
                    FechaPago = reader["fecha_pago"] != DBNull.Value ? ((DateTime)reader["fecha_pago"]).ToString("dd/MM/yyyy") : "",
                    Concepto = reader["concepto"].ToString() ?? "",
                    NumeroDocumentoPago = reader["numero_documento_pago"].ToString() ?? "",
                    NumeroCuota = reader["numero_cuota"] != DBNull.Value ? (int)reader["numero_cuota"] : 0,
                    Importe = reader["importe"] != DBNull.Value ? (decimal)reader["importe"] : 0,
                    MontoPagado = reader["monto_pagado"] != DBNull.Value ? (decimal)reader["monto_pagado"] : 0
                });
            }

            // Agrupar por periodo
            var obligacionesPorPeriodo = obligacionesPagadas
                .GroupBy(o => o.Periodo)
                .Select(g => new ObligacionPorPeriodoDTO
                {
                    Periodo = g.Key,
                    Pagos = g.Select(o => new ObligacionPagadaDTO
                    {
                        FechaPago = o.FechaPago,
                        Concepto = o.Concepto,
                        NumeroDocumentoPago = o.NumeroDocumentoPago,
                        NumeroCuota = o.NumeroCuota,
                        Importe = o.Importe,
                        MontoPagado = o.MontoPagado
                    }).ToList()
                }).ToList();

            return obligacionesPorPeriodo;
        }

        public async Task<Boolean> setImagenPago(ImagenPagoDto imagenPagoDto)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            var status = false;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
              connection.Open();

              string dml = $@"UPDATE pagos SET imagen = @imagen WHERE id_pago = @idPago";

              using (NpgsqlCommand cmd = new NpgsqlCommand(dml, connection))
              {
                  cmd.Parameters.AddWithValue("@idPago", imagenPagoDto.idPago);
                  cmd.Parameters.AddWithValue("@imagen", imagenPagoDto.imagen);
                  try
                  {
                      var result = cmd.ExecuteNonQuery();
                      status = true;
                  }
                  catch (Exception ex)
                  {
                     throw new Exception("error al actualizar");
                  }
              }
            }

            return status;
        }

        public async Task<List<AlumnoDTO>> getAlumnoPorSede(SedePaginadoDTO listaAlumno)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            var listaAlumnos = new List<AlumnoDTO>([]);
            int pagina = 0;

            if(listaAlumno.pagina > 1){
                pagina = (listaAlumno.pagina - 1) * listaAlumno.itemsPorPagina;
            }
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * from listar_alumnos_sede_paginado(@codigoSede, @pagina, @itemsPorPagina)", connection);
            cmd.Parameters.AddWithValue("codigoSede", listaAlumno.codigoSede);
            cmd.Parameters.AddWithValue("Pagina", pagina);
            cmd.Parameters.AddWithValue("itemsPorPagina", listaAlumno.itemsPorPagina);
            using NpgsqlDataReader reader = cmd.ExecuteReader();


            while (reader.Read())
            {
                   listaAlumnos.Add(new AlumnoDTO {
                    id_alumno =  Int32.Parse(reader["idalumno"].ToString() ?? "0"),
                    codigoAlumno = reader["codigoalumno"].ToString() ?? "",
                    nombre = reader["nombre_alumno"].ToString() ?? "",
                    apellidoPaterno = reader["apellido_paterno_alumno"].ToString() ?? "",
                    apellidoMaterno = reader["apellido_materno_alumno"].ToString() ?? "",
                    dni = reader["dni_alumno"].ToString() ?? "",
                    correo = reader["correo_alumno"].ToString() ?? "",
                    telefono = reader["telefono_alumno"].ToString() ?? "",
                    direccion = reader["direccion_alumno"].ToString() ?? "",
                    fotoPerfil = reader["foto_perfil_alumno"].ToString() ?? "",
                    genero = reader["genero_alumno"].ToString() ?? "",
                    tipoAlumno = reader["tipoalumno"].ToString() ?? "",
                    observaciones = reader["observaciones_alumno"].ToString() ?? "",
                    apoderado = reader["apoderado_alumno"].ToString() ?? "",
                    fechaNacimiento = reader["fecha_nacimiento_alumno"].ToString() ?? ""  ,
                    idGrado = Int32.TryParse(reader["id_grado_alumno"].ToString(), out var idGrado) ? idGrado : 0,
                    habilitadoPrueba = Boolean.TryParse(reader["habilitado_prueba_alumno"].ToString(), out var habilitadoPrueba) && habilitadoPrueba,
                    total = Int32.Parse(reader["total_resultados"].ToString() ?? "0")
                });
                
            }
            return listaAlumnos;
        }

        public async Task<List<AlumnoDTO>> filtrarAlumno(FiltroAlumnoDTO filtroAlumno)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            var listaAlumnos = new List<AlumnoDTO>([]);
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();
            int pagina = 0;

            if(filtroAlumno.pagina > 1){
                pagina = (filtroAlumno.pagina - 1) * filtroAlumno.itemsPorPagina;
            }


            using NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * from buscar_alumnos_paginado(@codigoSede, @filtro, @pagina, @itemsPorPagina)", connection);
            cmd.Parameters.AddWithValue("codigoSede", filtroAlumno.codigoSede);
            cmd.Parameters.AddWithValue("filtro", filtroAlumno.filtro);
            cmd.Parameters.AddWithValue("pagina", pagina);
            cmd.Parameters.AddWithValue("itemsPorPagina", filtroAlumno.itemsPorPagina);

            using NpgsqlDataReader reader = cmd.ExecuteReader();


            while (reader.Read())
            {
                   listaAlumnos.Add(new AlumnoDTO {
                    id_alumno =  Int32.Parse(reader["idalumno"].ToString() ?? "0"),
                    codigoAlumno = reader["codigoalumno"].ToString() ?? "",
                    nombre = reader["nombre_alumno"].ToString() ?? "",
                    apellidoPaterno = reader["apellido_paterno_alumno"].ToString() ?? "",
                    apellidoMaterno = reader["apellido_materno_alumno"].ToString() ?? "",
                    dni = reader["dni_alumno"].ToString() ?? "",
                    correo = reader["correo_alumno"].ToString() ?? "",
                    telefono = reader["telefono_alumno"].ToString() ?? "",
                    direccion = reader["direccion_alumno"].ToString() ?? "",
                    fotoPerfil = reader["foto_perfil_alumno"].ToString() ?? "",
                    genero = reader["genero_alumno"].ToString() ?? "",
                    tipoAlumno = reader["tipoalumno"].ToString() ?? "",
                    observaciones = reader["observaciones_alumno"].ToString() ?? "",
                    apoderado = reader["apoderado_alumno"].ToString() ?? "",
                    fechaNacimiento = reader["fecha_nacimiento_alumno"].ToString() ?? ""  ,
                    idGrado = Int32.TryParse(reader["id_grado_alumno"].ToString(), out var idGrado) ? idGrado : 0,
                    habilitadoPrueba = Boolean.TryParse(reader["habilitado_prueba_alumno"].ToString(), out var habilitadoPrueba) && habilitadoPrueba,
                    total = Int32.Parse(reader["total_resultados"].ToString() ?? "0")
                });
                
            }
            return listaAlumnos;
        }

        public async Task<Boolean> registrarUsuarioAlumno(AlumnoRegistrarDTO alumnoRegistrarDto)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

             if(string.IsNullOrEmpty(alumnoRegistrarDto.numeroDocumento)){
                throw new ArgumentException("El numero de documento es obligatorio");
            }
            
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(@"CALL public.insertar_usuario_alumno(@correo,
                 @nombre, @ap, @am, @telefono, @dni, @codigosede, @fechanacimiento, @direccion,
                 @foto, @genero, @talumno, @observacion, @apoderado, @tinstitucion, @gradoalumno, @habilitadopruebaalumno)", connection))
                {

                    command.Parameters.AddWithValue("correo", alumnoRegistrarDto.correo);
                    command.Parameters.AddWithValue("nombre", alumnoRegistrarDto.nombreUsuario);
                    command.Parameters.AddWithValue("ap", alumnoRegistrarDto.apellidoPaterno);
                    command.Parameters.AddWithValue("am", alumnoRegistrarDto.apellidoMaterno);
                    command.Parameters.AddWithValue("telefono", alumnoRegistrarDto.telefono);
                    command.Parameters.AddWithValue("dni", alumnoRegistrarDto.numeroDocumento);
                    command.Parameters.AddWithValue("codigosede", alumnoRegistrarDto.codigoSede);
                    command.Parameters.AddWithValue("fechanacimiento", DateTime.Parse(alumnoRegistrarDto.fechaNacimiento));
                    command.Parameters.AddWithValue("direccion", alumnoRegistrarDto.direccion);
                    command.Parameters.AddWithValue("foto", alumnoRegistrarDto.fotoPerfil);
                    command.Parameters.AddWithValue("genero", alumnoRegistrarDto.genero);
                    command.Parameters.AddWithValue("talumno", alumnoRegistrarDto.tipoAlumno);
                    command.Parameters.AddWithValue("observacion", alumnoRegistrarDto.observaciones);
                    command.Parameters.AddWithValue("apoderado", alumnoRegistrarDto.apoderado);
                    command.Parameters.AddWithValue("tinstitucion", alumnoRegistrarDto.tipoInstitucion);
                    command.Parameters.AddWithValue("gradoalumno", alumnoRegistrarDto.idGrado);
                    command.Parameters.AddWithValue("habilitadopruebaalumno", alumnoRegistrarDto.habilitadoPrueba);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                       throw new ArgumentException("error al registrar");
                    }
                }
            }

            return true;
        }

        public async Task<Boolean> actualizarUsuarioAlumno(AlumnoRegistrarDTO alumnoRegistrarDto)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            if(string.IsNullOrEmpty(alumnoRegistrarDto.numeroDocumento)){
                throw new ArgumentException("El numero de documento es obligatorio");
            }
            
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(@"CALL public.actualizar_usuario_alumno(@correo, @contraseña,
                 @nombre, @ap, @am, @telefono, @dni, @fechanacimiento, @direccion,
                 @foto, @genero, @talumno, @observacion, @apoderado, @tinstitucion, @gradoalumno, @habilitadopruebaalumno)", connection))
                {

                    command.Parameters.AddWithValue("correo", alumnoRegistrarDto.correo);
                    command.Parameters.AddWithValue("contraseña", alumnoRegistrarDto.contraseña ?? "");
                    command.Parameters.AddWithValue("nombre", alumnoRegistrarDto.nombreUsuario);
                    command.Parameters.AddWithValue("ap", alumnoRegistrarDto.apellidoPaterno);
                    command.Parameters.AddWithValue("am", alumnoRegistrarDto.apellidoMaterno);
                    command.Parameters.AddWithValue("telefono", alumnoRegistrarDto.telefono);
                    command.Parameters.AddWithValue("dni", alumnoRegistrarDto.numeroDocumento);
                    command.Parameters.AddWithValue("fechanacimiento", DateTime.Parse(alumnoRegistrarDto.fechaNacimiento));
                    command.Parameters.AddWithValue("direccion", alumnoRegistrarDto.direccion);
                    command.Parameters.AddWithValue("foto", alumnoRegistrarDto.fotoPerfil);
                    command.Parameters.AddWithValue("genero", alumnoRegistrarDto.genero);
                    command.Parameters.AddWithValue("talumno", alumnoRegistrarDto.tipoAlumno);
                    command.Parameters.AddWithValue("observacion", alumnoRegistrarDto.observaciones);
                    command.Parameters.AddWithValue("apoderado", alumnoRegistrarDto.apoderado);
                    command.Parameters.AddWithValue("tinstitucion", alumnoRegistrarDto.tipoInstitucion);
                    command.Parameters.AddWithValue("gradoalumno", alumnoRegistrarDto.idGrado);
                    command.Parameters.AddWithValue("habilitadopruebaalumno", alumnoRegistrarDto.habilitadoPrueba);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                       throw new Exception("error al actualizar");
                    }
                }
            }

            return true;
        }

        public async Task<Boolean> eliminarUsuarioAlumno(string numeroDocumento)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(@"CALL eliminar_usuario_alumno(@numerodocumento)", connection))
                {

                    command.Parameters.AddWithValue("numerodocumento", numeroDocumento);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                       throw new Exception("error al eliminar");
                    }
                }
            }

            return true;
        }
        
        public async Task<Boolean> AddDocument(DocumentoAddDTO documentoAddDto)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            var status = false;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
              await connection.OpenAsync();

            string query = @"INSERT INTO documentos
                            (""ID_CATEGORIA_DOCUMENTO"", ""STATUS"", ""TITULO"", ""DESCRIPCION"", ""ENLACE"", ""SECUENCIA"",
                            ""DATE_CREATED"", ""TIPO_DOCUMENTO"", ""MAS_BUSCADOS"", ""SECUENCIA_MAS_BUSCADA"", ""DOCUMENTO_VER"",
                            ""INTERNO"", ""FECHA_ACTUALIZACION"", ""FECHA_INICIO"", ""FECHA_FIN"", ""DOCUMENTO_DESCARGA"",
                            ""NOMBRE_DOCUMENTO"", ""TYPE"")
                            VALUES
                            (@idCategoriaDocumento, @status, @titulo, @descripcion, @enlace, @secuencia, @dateCreated,
                            @tipoDocumento, @masBuscados, @secuenciaMasBuscada, @documentoVer, @interno, @fechaActualizacion,
                            @fechaInicio, @fechaFin, @documentoDescarga, @nombreDocumento, @type)";

              using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
              {
                  cmd.Parameters.AddWithValue("@idCategoriaDocumento", documentoAddDto.IdCategoriaDocumento);
                  cmd.Parameters.AddWithValue("@status", documentoAddDto.Status ?? "published");
                  cmd.Parameters.AddWithValue("@titulo", documentoAddDto.Titulo ?? "Nuevo Documento");
                  cmd.Parameters.AddWithValue("@descripcion", (object?)documentoAddDto.Descripcion ?? DBNull.Value);
                  cmd.Parameters.AddWithValue("@enlace", (object?)documentoAddDto.Enlace ?? DBNull.Value);
                  cmd.Parameters.AddWithValue("@secuencia", documentoAddDto.Secuencia ?? 0);
                  cmd.Parameters.AddWithValue("@dateCreated", documentoAddDto.DateCreated ?? DateTime.Now);
                  cmd.Parameters.AddWithValue("@tipoDocumento", documentoAddDto.TipoDocumento ?? "pdf");
                  cmd.Parameters.AddWithValue("@masBuscados", documentoAddDto.MasBuscados);
                  cmd.Parameters.AddWithValue("@secuenciaMasBuscada", documentoAddDto.SecuenciaMasBuscada ?? 0);
                  cmd.Parameters.AddWithValue("@documentoVer", documentoAddDto.Documento ?? "");
                  cmd.Parameters.AddWithValue("@interno", documentoAddDto.Interno);
                  cmd.Parameters.AddWithValue("@fechaActualizacion", documentoAddDto.FechaActualizacion ?? DateTime.Now);
                  cmd.Parameters.AddWithValue("@fechaInicio", documentoAddDto.FechaInicio ?? DateTime.Now);
                  cmd.Parameters.AddWithValue("@fechaFin", documentoAddDto.FechaFin ?? DateTime.Now);
                  cmd.Parameters.AddWithValue("@documentoDescarga", documentoAddDto.DocumentoDescarga ?? "");
                  cmd.Parameters.AddWithValue("@nombreDocumento", documentoAddDto.Titulo ?? "Nuevo Documento");
                  cmd.Parameters.AddWithValue("@type", documentoAddDto.Type ?? "application/pdf");

                  try
                  {
                      var result = cmd.ExecuteNonQuery();
                      status = true;
                  }
                  catch (Exception ex)
                  {
                     throw new Exception("Error al agregar el documento", ex);
                  }
              }
            }

            return status;
        }

        public async Task<List<GradoDTO>> GetGrados()
        {
            var grados = new List<GradoDTO>();

            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"SELECT ""ID_GRADO"", ""NUMERO_GRADO"", ""DESCRIPCION_GRADO"", ""NIVEL_EDUCATIVO"" FROM public.grado";

                using (var cmd = new NpgsqlCommand(query, connection))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        grados.Add(new GradoDTO
                        {
                            IdGrado = reader.GetInt32(0),
                            NumeroGrado = reader.GetInt32(1),
                            DescripcionGrado = reader.GetString(2),
                            NivelEducativo = reader.GetString(3)
                        });
                    }
                }
            }

            return grados;
        }
    }
}