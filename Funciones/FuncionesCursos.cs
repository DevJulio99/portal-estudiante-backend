using System.Globalization;
using MyPortalStudent.Domain;
using MyPortalStudent.Domain.Ifunciones;
using Npgsql;
using Dapper;

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
            await using var connection = new NpgsqlConnection(connectionString); // C# 8 using declaration
            
            const string sql = @"
                SELECT 
                    id_alumno, codigo_alumno AS CodigoAlumno, nombre, 
                    apellido_paterno AS ApellidoPaterno, apellido_materno AS ApellidoMaterno,
                    dni, correo, fecha_nacimiento AS FechaNacimiento, telefono, direccion,
                    foto_perfil AS FotoPerfil, genero, tipo_alumno AS TipoAlumno, observaciones,
                    apoderado
                FROM public.alumno";

            var alumnos = await connection.QueryAsync<AlumnoDTO>(sql);
            return alumnos.AsList();
        }

        public async Task<Boolean> existeAlumno(string? numDocUsuario)
        {
            var connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            await using var connection = new NpgsqlConnection(connectionString);
            // Usamos EXISTS para que la base de datos nos devuelva directamente un booleano.
            const string sql = "SELECT EXISTS (SELECT 1 FROM alumno WHERE dni = @Dni)";
            return await connection.ExecuteScalarAsync<bool>(sql, new { Dni = numDocUsuario });
        }

        public async Task<string> asistenciasPorCursoAlumno(AsistenciaCursoAlumnoDTO asistenciaCursoAlumnoDto)
        {
            var connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            await using var connection = new NpgsqlConnection(connectionString);
            const string sql = @"SELECT * FROM asistencias_por_curso_alumno(
                                     @idAlumno, @anio, @inicioPeriodo, @finalPeriodo, @codCurso, @estadoAsistencia)";
            
            var parameters = new
            {
                idAlumno = asistenciaCursoAlumnoDto.idAlumno,
                anio = asistenciaCursoAlumnoDto.anio,
                inicioPeriodo = asistenciaCursoAlumnoDto.inicioPeriodo,
                finalPeriodo = asistenciaCursoAlumnoDto.finalPeriodo,
                codCurso = asistenciaCursoAlumnoDto.codigoCurso,
                estadoAsistencia = asistenciaCursoAlumnoDto.estadoAsistencia
            };

            var result = await connection.QueryFirstOrDefaultAsync<string>(sql, parameters);
            return result ?? "0";
        }

        public async Task<List<PerfilDTO>> getAlumnosId(string? numDocUsuario)
        {
            var connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            await using var connection = new NpgsqlConnection(connectionString);

            const string sql = @"
                SELECT
                    codigo_alumno AS CodAlumno,
                    apellido_paterno AS ApePatImag,
                    apellido_materno AS ApeMatImag,
                    dni AS DocumenIdentida,
                    tipo_alumno AS DesTipoAlumno,
                    fecha_nacimiento AS FechaNacimiento,
                    genero AS Sexo,
                    telefono,
                    direccion,
                    nombre || ' ' || apellido_paterno || ' ' || apellido_materno AS FullName,
                    correo AS CorreoPersonal,
                    p.codigo_periodo AS codPeriodoActual,
                    COALESCE(s.codigo_subperiodo, '') AS codSubperiodoActual
                FROM alumno
                LEFT JOIN 
                PeriodoAcademico p ON CURRENT_DATE BETWEEN
	            p.fecha_inicio AND p.fecha_fin AND p.tipo_periodo = 'Año'
                LEFT JOIN 
                subperiodos s ON p.id_periodo = s.id_periodo 
                    AND CURRENT_DATE BETWEEN s.fecha_inicio AND s.fecha_fin
                WHERE dni = @NumDocUsuario";

            var alumnos = await connection.QueryAsync<PerfilDTO>(sql, new { NumDocUsuario = numDocUsuario });
            return alumnos.AsList();
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

        public async Task<List<ReporteMatriculaColegioDTO>> getCursosColegio(int idAlum, int anio, string codPeriodo)
        {
            var connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            await using var connection = new NpgsqlConnection(connectionString);

            //const string sql = "SELECT * FROM public.obtener_reporte_matricula_colegio(@IdAlum, @Anio)";
            const string sql = "SELECT * FROM public.obtener_reporte_matricula_colegio_periodo(@IdAlum, @Anio, @CodPeriodo)";
            var queryResult = await connection.QueryAsync(sql, new { IdAlum = idAlum, Anio = anio, CodPeriodo = codPeriodo});

            var listaReporteColegio = new List<ReporteMatriculaColegioDTO>([]);

            foreach (var reader in queryResult)
            {
                var fechaActual = DateTime.Now;
                var fechaActualTiempo = fechaActual.Ticks;
                var fechaInicioString = reader.fecha_inicio.ToString();
                var fechaFinString = reader.fecha_fin.ToString();
                var fechaIniciotiempo = DateTime.ParseExact(fechaInicioString, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                var fechaFintiempo = DateTime.ParseExact(fechaFinString, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                var actualBimestre = fechaActualTiempo >= fechaIniciotiempo.Ticks && fechaActualTiempo <= fechaFintiempo.Ticks;
                var codCurso = reader.cod_cursos_matriculados.ToString();
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
                       codCurso = codCurso ?? "",
                       descCurso = reader.cursos_matriculados,
                       codigoPeriodoAcademico = reader.codigo_periodo,
                       periodo = reader.periodo_academico,
                       salon = "",
                       seccion = reader.seccion,
                       docente = [
                       new DocenteCursoDTO {
                           nombresDocentes = reader.docente_nombre,
                           apellidoPaternoDocente = "",
                           apellidoMaternoDocente = "",
                           emailDocente = reader.docente_email,
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
                       notaFinal = reader.nota_promedio_final != null ? (float)reader.nota_promedio_final : 0.0f,
                       tieneHorario = false,
                       grado = reader.grado,
                       nivel = reader.nivel,
                       periodoAcademico = reader.periodo_academico,
                       fechaInicio = fechaInicioString,
                       fechaFin = fechaFinString,
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

        public async Task<List<NotasxBimestreDTO>> getNotasxBimestre(int idAlum, int anio, string codCurso, string codSubperiodo)
        {
            var connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            await using var connection = new NpgsqlConnection(connectionString);
            
            const string sql = @"
                SELECT 
                    alumno,
                    apellido_paterno AS ApellidoPaterno,
                    apellido_materno AS ApellidoMaterno,
                    cod_curso as codigoCurso,
                    descripcion_curso AS DescripcionCurso,
                    cod_periodo AS CodigoPeriodo,
                    descripcion_periodo AS DescripcionPeriodo,
                    cod_subperiodo AS CodigoSubperiodo,
                    descripcion_subperiodo AS DescripcionSubperiodo,
                    nota,
                    peso,
                    tipo_nota AS TipoNota
                FROM obtener_notas_por_curso_subperiodo(@IdAlum, @Anio, @CodCurso, @CodSubperiodo)";
            var parameters = new { IdAlum = idAlum, Anio = anio, CodCurso = codCurso, CodSubperiodo = codSubperiodo};
            
            var notas = await connection.QueryAsync<NotasxBimestreDTO>(sql, parameters);
            return notas.AsList();
        }

        public async Task<List<PagoDTO>> getPagosPorAlumno(int idAlumno, int anio)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);

            const string sql = "SELECT * FROM get_pagos_por_alumno(@IdAlumno, @Anio)";
            
            // Dapper mapeará automáticamente las columnas a las propiedades del DTO.
            // id_pago -> IdPago, documento_pago -> DocumentoPago, f_vencimiento -> FechaVencimiento, etc.
            var pagos = await connection.QueryAsync<PagoDTO>(sql, new { IdAlumno = idAlumno, Anio = anio });

            return pagos.AsList();
        }

        public async Task<List<ResumenPagosDTO?>> GetResumenPagosPorAlumno(int idAlumno, int anio)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using var cmd = new NpgsqlCommand("SELECT * FROM get_resumen_pagos_por_alumno(@id_alumno, @anio)", connection);
            cmd.Parameters.AddWithValue("id_alumno", idAlumno);
            cmd.Parameters.AddWithValue("anio", anio);

            using var reader = await cmd.ExecuteReaderAsync();

            var resumenList = new List<ResumenPagosDTO>();

            while (await reader.ReadAsync())
            {
                resumenList.Add(new ResumenPagosDTO
                {
                    IdAlumno = (int)reader["id_alumno"],
                    PagosVencidos = (int)reader["cantidad_vencidos"],
                    PagosPorVencer = (int)reader["cantidad_por_vencer"],
                    PagosATiempo = (int)reader["cantidad_a_tiempo"],
                    MontoTotalPendiente = (decimal)reader["monto_total_pendiente"]
                });
            }

            return resumenList;
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
                    command.Parameters.AddWithValue("gradoalumno", (object)alumnoRegistrarDto.idGrado ?? DBNull.Value);
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
                    command.Parameters.AddWithValue("gradoalumno", (object)alumnoRegistrarDto.idGrado ?? DBNull.Value);
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

        public async Task<List<GradoDTO>> GetGrados(string tipoInstitucion)
        {
            var grados = new List<GradoDTO>();
            
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            await using var connection = new NpgsqlConnection(connectionString);

            const string query = @"SELECT ""ID_GRADO"" AS IdGrado, ""NUMERO_GRADO"" AS NumeroGrado, ""DESCRIPCION_GRADO"" AS DescripcionGrado, ""NIVEL_EDUCATIVO"" AS NivelEducativo FROM public.grado WHERE tipo_institucion ILIKE @Institucion";

            var result = await connection.QueryAsync<GradoDTO>(query, new { Institucion = tipoInstitucion });
            return result.AsList();
        }

        public async Task<List<CursoListarDTO>> ListarCursosPorSede(SedePaginadoDTO listaCurso)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            var listaCursos = new List<CursoListarDTO>([]);
            int pagina = 0;

            if(listaCurso.pagina > 1){
                pagina = (listaCurso.pagina - 1) * listaCurso.itemsPorPagina;
            }
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * from listar_cursos_sede_paginado(@codigoSede, @pagina, @itemsPorPagina)", connection);
            cmd.Parameters.AddWithValue("codigoSede", listaCurso.codigoSede);
            cmd.Parameters.AddWithValue("pagina", pagina);
            cmd.Parameters.AddWithValue("itemsPorPagina", listaCurso.itemsPorPagina);
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                listaCursos.Add(new CursoListarDTO
                {
                    IdCurso  =  Int32.Parse(reader["idcurso"].ToString() ?? "0"),
                    CodigoCurso = reader["codigocurso"].ToString() ?? "",
                    Descripcion = reader["descripcioncurso"].ToString() ?? string.Empty,
                    Creditos = decimal.Parse(reader["creditoscurso"].ToString() ?? "0"),
                    Modalidad = reader["modalidadcurso"].ToString() ?? string.Empty,
                    Nivel = reader["nivelcurso"].ToString() ?? string.Empty,
                    Total = Int32.Parse(reader["total_resultados"].ToString() ?? "0")
                });
                
            }
            return listaCursos;
        }

        public async Task<List<CursoListarDTO>> FiltrarCurso(FiltroCursoDTO filtroCurso)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            var listaCursos = new List<CursoListarDTO>([]);
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            int pagina = 0;

            if(filtroCurso.pagina > 1){
                pagina = (filtroCurso.pagina - 1) * filtroCurso.itemsPorPagina;
            }


            using NpgsqlCommand cmd = new NpgsqlCommand($@"SELECT * from buscar_cursos_paginado(@codigoSede, @filtro, @pagina, @itemsPorPagina)", connection);
            cmd.Parameters.AddWithValue("codigoSede", filtroCurso.codigoSede);
            cmd.Parameters.AddWithValue("filtro", filtroCurso.filtro);
            cmd.Parameters.AddWithValue("pagina", pagina);
            cmd.Parameters.AddWithValue("itemsPorPagina", filtroCurso.itemsPorPagina);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                   listaCursos.Add(new CursoListarDTO {
                    IdCurso  =  Int32.Parse(reader["idcurso"].ToString() ?? "0"),
                    CodigoCurso = reader["codigocurso"].ToString() ?? "",
                    Descripcion = reader["descripcioncurso"].ToString() ?? string.Empty,
                    Creditos = decimal.Parse(reader["creditoscurso"].ToString() ?? "0"),
                    Modalidad = reader["modalidadcurso"].ToString() ?? string.Empty,
                    Nivel = reader["nivelcurso"].ToString() ?? string.Empty,
                    Total = Int32.Parse(reader["total_resultados"].ToString() ?? "0")
                });                
            }
            return listaCursos;
        }

        public async Task<Boolean> RegistrarCurso(CursoRegistrarDTO cursoRegistrarDto)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

             if(string.IsNullOrEmpty(cursoRegistrarDto.DescripcionCurso)){
                throw new ArgumentException("El nombre del curso es obligatorio");
            }
            
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(@"CALL public.insertar_curso(@descripcion, @creditos,
                @modalidad, @nivel, @codSede)", connection))
                {
                    command.Parameters.AddWithValue("descripcion", cursoRegistrarDto.DescripcionCurso);
                    command.Parameters.AddWithValue("creditos", cursoRegistrarDto.Creditos);
                    command.Parameters.AddWithValue("modalidad", cursoRegistrarDto.Modalidad);
                    command.Parameters.AddWithValue("nivel", cursoRegistrarDto.Nivel);
                    command.Parameters.AddWithValue("codSede", cursoRegistrarDto.CodigoSede);

                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (PostgresException ex)
                    {
                       throw new ArgumentException(ex.MessageText);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Error al registrar el curso: {ex.Message}");
                    }
                }
            }

            return true;
        }

        public async Task<Boolean> ActualizarCurso(CursoActualizarDTO cursoActualizarDto)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

             if(string.IsNullOrEmpty(cursoActualizarDto.DescripcionCurso)){
                throw new ArgumentException("El nombre del curso es obligatorio");
            }

            if (cursoActualizarDto.IdCurso <= 0)
            {
                throw new ArgumentException("El id del curso debe ser un número positivo");
            }
            
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(@"CALL public.actualizar_curso(@id, @descripcion, @creditos,
                @modalidad, @nivel, @codSede)", connection))
                {
                    command.Parameters.AddWithValue("id", cursoActualizarDto.IdCurso);
                    command.Parameters.AddWithValue("descripcion", cursoActualizarDto.DescripcionCurso);
                    command.Parameters.AddWithValue("creditos", cursoActualizarDto.Creditos);
                    command.Parameters.AddWithValue("modalidad", cursoActualizarDto.Modalidad);
                    command.Parameters.AddWithValue("nivel", cursoActualizarDto.Nivel);
                    command.Parameters.AddWithValue("codSede", cursoActualizarDto.CodigoSede);

                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (PostgresException ex)
                    {
                       throw new ArgumentException(ex.MessageText);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Error al actualizar el curso: {ex.Message}");
                    }
                }
            }

            return true;
        }

        public async Task<Boolean> EliminarCurso(int idCurso)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            if (idCurso <= 0)
            {
                throw new ArgumentException("El id del curso debe ser un número positivo");
            }
            
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(@"CALL eliminar_curso(@idCurso)", connection))
                {

                    command.Parameters.AddWithValue("idCurso", idCurso);

                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (PostgresException ex)
                    {
                       throw new ArgumentException(ex.MessageText);
                    }
                    catch (Exception ex)
                    {
                       throw new InvalidOperationException($"Error al eliminar el curso: {ex.Message}");
                    }
                }
            }

            return true;
        }

        public async Task<List<ReporteMatriculaColegioDTO>> getCursosAlumno(int idAlumno)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            await using var connection = new NpgsqlConnection(connectionString);

            const string sql = @"
                SELECT DISTINCT ON (mc.id_seccion)
                    c.codigo_curso AS CodCurso, 
                    c.descripcion_curso AS DescCurso, 
                    c.modalidad,
                    m.id_periodo as idPeriodo,
                    COALESCE(sp.codigo_subperiodo, pa.codigo_periodo) AS CodigoPeriodoAcademico,
                    COALESCE(sp.descripcion_subperiodo, pa.descripcion_periodo) AS Periodo,
                    COALESCE(sp.fecha_inicio, pa.fecha_inicio) AS FechaInicio,
                    COALESCE(sp.fecha_fin, pa.fecha_fin) AS FechaFin,
                    g.""DESCRIPCION_GRADO"" AS grado,
	                g.""NIVEL_EDUCATIVO"" AS nivel,
	                au.descripcion_aula AS salon,
	                sec.codigo_seccion,
	                sec.descripcion AS seccion,
	                sec.ciclo,
                    doc.nombre || ' ' || doc.apellido_paterno || ' ' || doc.apellido_materno AS nombreDocente,
	                doc.correo as correoDocente
                FROM matricula_curso mc
                INNER JOIN matricula m ON mc.id_matricula = m.id_matricula
                INNER JOIN grado g ON m.id_grado = g.""ID_GRADO""
                INNER JOIN alumno a ON a.id_alumno = m.id_alumno
                INNER JOIN curso c ON mc.id_curso = c.id_curso
                LEFT JOIN detalleseccionasignada dsa ON dsa.id_seccion = mc.id_seccion
                LEFT JOIN docente doc ON dsa.id_docente = doc.id_docente
                LEFT JOIN seccion sec ON dsa.id_seccion = sec.id_seccion
                LEFT JOIN aula au ON dsa.id_aula = au.id_aula
                --Si el alumno es de tipo 'c' -> se une a subperiodos
                LEFT JOIN LATERAL (
                    SELECT *
                    FROM subperiodos sp
                    WHERE a.tipo_institucion ILIKE 'c'
                      AND sp.id_periodo = m.id_periodo
                      AND (
                          CURRENT_DATE BETWEEN sp.fecha_inicio AND sp.fecha_fin
                          OR sp.fecha_inicio > CURRENT_DATE
                      )
                    ORDER BY sp.fecha_inicio
                    LIMIT 1
                ) sp ON TRUE
                --Si el alumno es de tipo 'i' -> se une a periodoacademico
                LEFT JOIN LATERAL (
                    SELECT *
                    FROM periodoacademico pa
                    WHERE a.tipo_institucion = 'i'
                      AND pa.id_periodo = m.id_periodo
                      AND (
                          CURRENT_DATE BETWEEN pa.fecha_inicio AND pa.fecha_fin
                          OR pa.fecha_inicio > CURRENT_DATE
                      )
                    ORDER BY pa.fecha_inicio
                    LIMIT 1
                ) pa ON TRUE
                WHERE m.id_alumno = @idAlumno AND m.activo = true
                ORDER BY mc.id_seccion, dsa.id_detalle";

            var cursos = await connection.QueryAsync<ReporteMatriculaColegioDTO>(sql, new { idAlumno });
            return cursos.AsList();
        }
    }
}