using MyPortalStudent.Domain.DTOs.CompetenciasGenerales;
using MyPortalStudent.Domain.IServices;
using MyPortalStudent.Domain;
using Npgsql;
using System.Data;
using System.Text.Json;

namespace MyPortalStudent.Services
{
    public class CompetenciasGeneralesService : ICompetenciasGeneralesService
    {
        public readonly IConfiguration _configuration;

        public CompetenciasGeneralesService(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Boolean> ExamenAleatorio(GenerarExamenDTO request)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            List<int> competenciasExistentes = new List<int>();
            int rowAffected = 0;

            if (request.idPostulante.Equals(0) || request.numeroPreguntas.Equals(0) || request.idCompetencia.Equals(0))
            {
                throw new Exception((request.idPostulante.Equals(0) ? "idPostulante" : request.numeroPreguntas.Equals(0) ? "numeroPreguntas" : "idCompetencia") + " no puede ser 0.");
            }
            var competencias = await listarCompetencias();
            foreach (var competencia in competencias) {
               competenciasExistentes.Add(competencia.id_compentencia);
            }

            if(!competenciasExistentes.Exists(x => x == request.idCompetencia)){
              throw new Exception("El id de competencia no existe");
            }
            
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString)){
               connection.Open();

               using(var cmd = new NpgsqlCommand("generar_examen_aleatorio_competencia_grado", connection)){
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_id_postulante", request.idPostulante);
                cmd.Parameters.AddWithValue("@p_numero_preguntas", request.numeroPreguntas);
                cmd.Parameters.AddWithValue("@p_id_competencia", request.idCompetencia);
                cmd.Parameters.AddWithValue("@p_id_grado", request.idGrado);
                cmd.Parameters.AddWithValue("@p_es_grupal", request.esGrupal);

                 try
                 {
                     rowAffected = cmd.ExecuteNonQuery();
                 }
                 catch (Exception ex)
                 {
                    
                
                 }
                }
                connection.Close();
            }
            

            
            

            return true;
        }

        public async Task<List<ExamenDTO>> listarExamen(int idPostulante, int idCompetencia)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand(
                $@"select e.*,p.*, c.""NOMBRE_COMPETENCIA"" from examen_generado 
                   e inner join pregunta p USING(""ID_PREGUNTA"") 
                   inner join competencia c on c.""ID_COMPETENCIA"" = p.""ID_COMPETENCIA""
                   where ""ID_POSTULANTE"" = {idPostulante}
                   AND e.""ID_COMPETENCIA"" = {idCompetencia}
                   order by e.""ORDEN_PREGUNTA""", connection);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var listaExamen = new List<ExamenDTO>([]);
            var listaGrupos = new List<ListaGruposDTO>([]);
            var numeroPregunta = 1;

            while (reader.Read())
            {
                var grupo = reader["GRUPO"]?.ToString() ?? "";
                var grupoRegistrado = listaGrupos.Exists(x => x.grupo == grupo);
                var indexGrupo = listaGrupos.FindIndex(x => x.grupo == grupo);
                var numeroPregunta_ = 1;

                if(grupo.Length > 0){
                    if(grupoRegistrado){
                        numeroPregunta_ = (listaGrupos[indexGrupo]?.numeroPreguntas ?? 1) + 1;
                        listaGrupos[indexGrupo] = new ListaGruposDTO{
                           grupo= reader["GRUPO"]?.ToString() ?? "",
                           numeroPreguntas = numeroPregunta_
                        };
                    }else {
                        listaGrupos.Add(new ListaGruposDTO{
                        grupo= reader["GRUPO"]?.ToString() ?? "",
                        numeroPreguntas = numeroPregunta_
                   });
                  }

                }

                listaExamen.Add(new ExamenDTO
                {
                    nombreCompetencia = reader["NOMBRE_COMPETENCIA"].ToString() ?? "",
                    examenGenerado = new ExamenGeneradoDTO
                    {
                        idExamenGenerado = (int)reader["ID_EXAMEN_GENERADO"],
                        idPostulante = (int)reader["ID_POSTULANTE"],
                        idPregunta = (int)reader["ID_PREGUNTA"],
                        ordenPregunta = numeroPregunta,
                        respuestaSeleccionada = reader["RESPUESTA_SELECCIONADA"].ToString() ?? "",
                        tiempoRespuesta = reader["TIEMPO_RESPUESTA"].ToString() ?? "",
                        completado = (bool)reader["COMPLETADO"],
                    },

                    preguntas = new PreguntaDTO2
                    {
                        tipoEvaluacion = reader["TIPO_EVALUACION"].ToString() ?? "",
                        grupo = reader["GRUPO"].ToString() ?? "",
                        textoTitulo = reader["TEXTO_TITULO"].ToString() ?? "",
                        textoSuperior = reader["TEXTO_SUPERIOR"].ToString() ?? "",
                        textoImagen = reader["TEXTO_IMAGEN"].ToString() ?? "",
                        textoInferior = reader["TEXTO_INFERIOR"].ToString() ?? "",
                        numeroPregunta = numeroPregunta,
                        // idArchivoCarga = Int32.Parse(reader["id_archivo_carga"]?.ToString() ?? "0"),
                        pregunta = reader["PREGUNTA"].ToString() ?? "",
                        opcionA = reader["OPCION_A"].ToString() ?? "",
                        opcionB = reader["OPCION_B"].ToString() ?? "",
                        opcionC = reader["OPCION_C"].ToString() ?? "",
                        opcionD = reader["OPCION_D"].ToString() ?? "",
                        respuestaCorrecta = reader["RESPUESTA_CORRECTA"].ToString() ?? "",
                        textoGrupo = $"Pregunta {numeroPregunta_}"
                        // idCompetencia = (int)reader["id_competencia"],
                    }

                });
                numeroPregunta++;
            }
            return listaExamen;
        }

        public async Task<List<CompetenciaDTO>> listarCompetencias()
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand(@"SELECT cp.*, ct.""TIEMPO_LIMITE"" FROM competencia cp inner join criterio_evaluacion ct USING(""ID_COMPETENCIA"") order by ""ID_COMPETENCIA""", connection);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var competencias = new List<CompetenciaDTO>([]);

            while (reader.Read())
            {
                competencias.Add(new CompetenciaDTO
                {
                    id_compentencia = Int32.Parse(reader["ID_COMPETENCIA"].ToString() ?? ""),
                    nombreCompetencia = reader["NOMBRE_COMPETENCIA"].ToString() ?? "",
                    pesoCompetencia = reader["PESO_COMPETENCIA"].ToString() ?? "",
                    descripcion = reader["DESCRIPCION"].ToString() ?? "",
                    fechaDisponibilidad = reader["FECHA_DISPONIBILIDAD"].ToString() ?? "",
                    fechaInicio = reader["FECHA_INICIO"].ToString() ?? "",
                    fechaFin = reader["FECHA_FIN"].ToString() ?? "",
                    horaInicio = reader["HORA_INICIO"].ToString() ?? "",

                    ordenCompetencia = reader["ORDEN_COMPETENCIA"].ToString() ?? "",
                    estadoCompetencia = reader["ESTADO_COMPETENCIA"].ToString() ?? "",
                    dependenciaCompetencia = reader["DEPENDENCIA_COMPETENCIA"].ToString() ?? "",
                    idEtapa = reader["ID_ETAPA"].ToString() ?? "",
                    idProceso = reader["ID_PROCESO"].ToString() ?? "",
                    tiempoLimite = reader["TIEMPO_LIMITE"].ToString() ?? "",
                    urlImagen = reader["URL_IMAGEN"].ToString() ?? "",
                });
            }

            return competencias;
        }

        public async Task<Boolean> ActualizarRespuesta(RespuestaReq request)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            List<string> parts = new List<string> { "a", "b", "c", "d" };

            if (request.idPostulante.Equals(0) || request.idPregunta.Equals(0))
            {
                throw new Exception((request.idPostulante.Equals(0) ? "idPostulante" : "idPregunta") + " no puede ser 0.");
            }


            if (!parts.Exists(x => x == request.respuestSeleccionada.ToLower()))
            {
                throw new Exception("La respuesta debe ser A,B,C o D");
            }

            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            NpgsqlCommand cmd = new NpgsqlCommand("actualizar_respuesta", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_respuesta_seleccionada", request.respuestSeleccionada.ToUpper());
            cmd.Parameters.AddWithValue("@p_id_postulante", request.idPostulante);
            cmd.Parameters.AddWithValue("@p_id_pregunta", request.idPregunta);

            connection.Open();
            int rowAffected = cmd.ExecuteNonQuery();
            connection.Close();

            return true;
        }
        
        public async Task<Boolean> CompetenciaCompleta(int idPostulante, int idCompetencia)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            Boolean completado = false;

            if (idPostulante.Equals(0) || idCompetencia.Equals(0))
            {
                throw new Exception((idPostulante.Equals(0) ? "idPostulante" : "idCompetencia") + " no puede ser 0.");
            }

            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand($@"select * from examen_generado where ""ID_POSTULANTE"" = {idPostulante} and ""ID_COMPETENCIA"" = {idCompetencia}", connection);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var examenes = new List<ExamenGeneradoDTO>([]);

            while (reader.Read())
            {
                examenes.Add(new ExamenGeneradoDTO(){
                        idExamenGenerado = (int)reader["ID_EXAMEN_GENERADO"],
                        idPostulante = (int)reader["ID_POSTULANTE"],
                        idPregunta = (int)reader["ID_PREGUNTA"],
                        ordenPregunta = (int)reader["ORDEN_PREGUNTA"],
                        respuestaSeleccionada = reader["RESPUESTA_SELECCIONADA"].ToString() ?? "",
                        tiempoRespuesta = reader["TIEMPO_RESPUESTA"].ToString() ?? "",
                        completado = (bool)reader["COMPLETADO"],
                });
            }

            completado = examenes.All(x => !string.IsNullOrEmpty(x.respuestaSeleccionada));

            return completado;
        }

        public async Task<List<PostulanteDTO>> listarPostulante(string? dniPostulante)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;

            if (string.IsNullOrEmpty(dniPostulante))
            {
                throw new Exception("dniPostulante es obligatorio.");
            }

            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand($@"select p.""ID_POSTULANTE"", p.""DNI"", p.""NOMBRE"", p.""APELLIDO"",
                                         p.""CORREO"", p.""CELULAR"", p.""ESTADO"", a.id_grado
                                         from alumno a
                                         inner join postulante p ON a.dni = p.""DNI""
                                         where p.""DNI"" = '{dniPostulante}'", connection);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var postulante = new List<PostulanteDTO>([]);

            while (reader.Read())
            {
                postulante.Add(new PostulanteDTO(){
                        idPostulante = (int)reader["ID_POSTULANTE"],
                        dni = reader["DNI"].ToString() ?? "",
                        nombre = reader["NOMBRE"].ToString() ?? "",
                        apellido = reader["APELLIDO"].ToString() ?? "",
                        correo = reader["CORREO"].ToString() ?? "",
                        celular = reader["CELULAR"].ToString() ?? "",
                        codigoPostulante = 0,
                        estado = (Boolean)reader["ESTADO"],
                        idGrado = (int)reader["id_grado"],
                });
            }


            return postulante;
        }

        public async Task<Boolean> registrarPostulante(RegistrarPostulanteDTO postulanteDto)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            Boolean status = false;

            var listaPostulante = await listarPostulante(postulanteDto.dni);

            if(listaPostulante.Count > 0){
                throw new Exception("El postulante ingresado ya esta registrado.");
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
              connection.Open();

              string dml = @"insert into postulante (""DNI"", ""NOMBRE"", ""APELLIDO"", ""CORREO"", ""ESTADO"") values (:DNI, :NOM, :APE, :COR, :EST)";

              using (NpgsqlCommand cmd = new NpgsqlCommand(dml, connection))
              {
                  cmd.Parameters.AddWithValue("DNI", postulanteDto.dni);
                  cmd.Parameters.AddWithValue("NOM", postulanteDto.nombre);
                  cmd.Parameters.AddWithValue("APE", postulanteDto.apellido);
                  cmd.Parameters.AddWithValue("COR", postulanteDto.correo);
                  cmd.Parameters.AddWithValue("EST", postulanteDto.estado);

                  try
                  {
                      var result = cmd.ExecuteNonQuery();
                  }
                  catch (Exception ex)
                  {
                     throw new Exception(ex.Message);
                  }
              }
            }
            return status;
        }

        public async Task<List<UltimaActividadDTO>> ultimaActividadXPostulante(int idPostulante, int idCompetencia)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand($@"select * from ultima_actividad where ""ID_POSTULANTE"" = '{idPostulante}' and ""ID_COMPETENCIA"" = {idCompetencia}", connection);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var lista = new List<UltimaActividadDTO>([]);

            while (reader.Read())
            {
                lista.Add(new UltimaActividadDTO(){
                    idPostulante = (int)reader["ID_POSTULANTE"],
                    idCompetencia = (int)reader["ID_COMPETENCIA"],
                    fechaUltimaActividad = reader["FECHA_ULTIMA_ACTIVIDAD"].ToString() ?? "",
                    horaUltimaActividad = reader["HORA_ULTIMA_ACTIVIDAD"].ToString() ?? "",
                });
            }

            return lista;
        }

        public async Task<Boolean> insertarActividadPostulante(UltimaActividadDTO ultimaActividadDto)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            Boolean status = true;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
              connection.Open();

              string dml = @"insert into ultima_actividad (""ID_POSTULANTE"",""ID_COMPETENCIA"", ""FECHA_ULTIMA_ACTIVIDAD"", ""HORA_ULTIMA_ACTIVIDAD"") values (:IDP, :IDC, :FULT, :HULT)";

              using (NpgsqlCommand cmd = new NpgsqlCommand(dml, connection))
              {
                  cmd.Parameters.AddWithValue("IDP", ultimaActividadDto.idPostulante);
                  cmd.Parameters.AddWithValue("IDC", ultimaActividadDto.idCompetencia);
                  cmd.Parameters.AddWithValue("FULT", DateTime.Parse(ultimaActividadDto.fechaUltimaActividad));
                  cmd.Parameters.AddWithValue("HULT", TimeSpan.Parse(ultimaActividadDto.horaUltimaActividad));

                  try
                  {
                      var result = cmd.ExecuteNonQuery();
                  }
                  catch (Exception ex)
                  {
                     status = false;
                  }
              }
            }
            return status;
        }

        public async Task<Boolean> actualizarActividadPostulante(UltimaActividadDTO ultimaActividadDto)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            Boolean status = true;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
              connection.Open();

              string dml = $@"update ultima_actividad set ""FECHA_ULTIMA_ACTIVIDAD"" = '{ultimaActividadDto.fechaUltimaActividad}',
                           ""HORA_ULTIMA_ACTIVIDAD"" = '{ultimaActividadDto.horaUltimaActividad}' where ""ID_POSTULANTE"" = {ultimaActividadDto.idPostulante} and ""ID_COMPETENCIA"" = {ultimaActividadDto.idCompetencia}";

              using (NpgsqlCommand cmd = new NpgsqlCommand(dml, connection))
              {
                  try
                  {
                      var result = cmd.ExecuteNonQuery();
                  }
                  catch (Exception ex)
                  {
                     status = false;
                  }
              }
            }
            return status;
        }

        public async Task<Boolean> registrarActividadPostulante(UltimaActividadDTO ultimaActividadDto)
        {
            var listaActividad = await ultimaActividadXPostulante(ultimaActividadDto.idPostulante, ultimaActividadDto.idCompetencia);
            Boolean status = false;

            if(listaActividad.Count > 0){
                status = await actualizarActividadPostulante(ultimaActividadDto);
            }

            if(listaActividad.Count <= 0){
                status = await insertarActividadPostulante(ultimaActividadDto);
            }

           return status;
        }

        public async Task<List<UltimaRespuestaExamenDTO>> getUltimaRespuestaExamen(int idPostulante, int idCompetencia)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            var lista = new List<UltimaRespuestaExamenDTO>([]);

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
              connection.Open();

              string dml = $@"select * from examen_generado 
                              where ""TIEMPO_RESPUESTA"" = (select max(""TIEMPO_RESPUESTA"") from examen_generado where ""ID_POSTULANTE"" = {idPostulante} and 
                              ""ID_COMPETENCIA"" = {idCompetencia})";

              using (NpgsqlCommand cmd = new NpgsqlCommand(dml, connection))
              {
                  try
                  {
                    using NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        lista.Add(new UltimaRespuestaExamenDTO{
                            idPregunta = (int)reader["ID_PREGUNTA"],
                            respuestaSelecciona = reader["RESPUESTA_SELECCIONADA"].ToString() ?? "",
                            tiempoRespuesta = DateTime.Parse(reader["TIEMPO_RESPUESTA"].ToString() ?? "").ToString("MM/dd/yyyy HH:mm:ss") ?? ""
                        });
                    }
                  }
                  catch (Exception ex)
                  {
                     
                  }
              }
            }
            return lista;
        }

        // public async Task<List<UltimaRespuestaExamenDTO>> getUltimaActividadPostulante(int idPostulante, int idCompetencia)
        // {
        //     var ultimaRespuestaExamen = await getUltimaRespuestaExamen(idPostulante, idCompetencia);
        //     //var listaUltimaActividad = await ultimaActividadXPostulante(idPostulante, idCompetencia);
        //     var tiempoUltimaActividad = "";
        //     var ultimaActividad = "";

        //     // if(listaUltimaActividad.Count > 0){
        //     //    var hora = TimeSpan.Parse(listaUltimaActividad[0].horaUltimaActividad);
        //     //    var date = DateTime.Parse($"{listaUltimaActividad[0].fechaUltimaActividad}");
        //     //    tiempoUltimaActividad = new DateTime(date.Year, date.Month, date.Day, hora.Hours, hora.Minutes, hora.Seconds).ToString();
        //     // }

        //     if(ultimaRespuestaExamen.Count > 0){
        //       if(!string.IsNullOrEmpty(tiempoUltimaActividad) && !string.IsNullOrEmpty(ultimaRespuestaExamen[0].tiempoRespuesta)){
        //           if(DateTime.Parse(tiempoUltimaActividad).Ticks > DateTime.Parse(ultimaRespuestaExamen[0].tiempoRespuesta).Ticks){
        //              ultimaActividad = DateTime.Parse(tiempoUltimaActividad).ToString();
        //           }else{
        //              ultimaActividad = DateTime.Parse(ultimaRespuestaExamen[0].tiempoRespuesta).ToString();
        //           }
        //       }

        //       if(string.IsNullOrEmpty(tiempoUltimaActividad) && !string.IsNullOrEmpty(ultimaRespuestaExamen[0].tiempoRespuesta)){
        //           ultimaActividad = DateTime.Parse(ultimaRespuestaExamen[0].tiempoRespuesta).ToString();
        //       }

        //       if(!string.IsNullOrEmpty(tiempoUltimaActividad) && string.IsNullOrEmpty(ultimaRespuestaExamen[0].tiempoRespuesta)){
        //           ultimaActividad = DateTime.Parse(tiempoUltimaActividad).ToString();
        //       }
        //     }

        //    return ultimaRespuestaExamen;
        // }

        public async Task<List<ListaEstadoCompetenciaDTO>> listarEstadoCompetencia(int idPostulante, int? idCompetencia)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            string queryDb = "";
            if(idCompetencia == null){
                queryDb = @$"select * from estado_competencia where ""ID_POSTULANTE"" = '{idPostulante}'";
            }else {
                queryDb = @$"select * from estado_competencia where ""ID_POSTULANTE"" = '{idPostulante}' and ""ID_COMPETENCIA"" = {idCompetencia}";
            }
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(queryDb, connection);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var lista = new List<ListaEstadoCompetenciaDTO>([]);

            while (reader.Read())
            {
                lista.Add(new ListaEstadoCompetenciaDTO(){
                    idPostulante = (int)reader["ID_POSTULANTE"],
                    idCompetencia = (int)reader["ID_COMPETENCIA"],
                    estado = reader["ESTADO"].ToString().ToUpper() ?? "",
                    tiempoIniciado = !string.IsNullOrEmpty(reader["TIEMPO_INICIADO"].ToString() ?? "") ? DateTime.Parse(reader["TIEMPO_INICIADO"].ToString()).ToString("MM/dd/yyyy HH:mm:ss") : "",
                    tiempoFinalizado = !string.IsNullOrEmpty(reader["TIEMPO_FINALIZADO"]?.ToString() ?? "") ? DateTime.Parse(reader["TIEMPO_FINALIZADO"].ToString()).ToString("MM/dd/yyyy HH:mm:ss") : ""
                });
            }

            return lista;
        }

        public async Task<Boolean> registrarEstadoCompetencia(EstadoCompetenciaDTO estadoCompetenciaDto)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            Boolean status = true;

            var listaEstados = await listarEstadoCompetencia(estadoCompetenciaDto.idPostulante, estadoCompetenciaDto.idCompetencia);

            if(listaEstados.Count > 0){
                throw new Exception("El estado para la competencia y postulante ya existe");
            }

            if(estadoCompetenciaDto.estado != null){
                if(estadoCompetenciaDto.estado.ToLower() != "i" && estadoCompetenciaDto.estado.ToLower() != "f"){
                 throw new Exception("El estado debe contener solo el valor de I o F");
                }
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
              connection.Open();

              string dml = @"insert into estado_competencia (""ID_COMPETENCIA"", ""ID_POSTULANTE"", ""ESTADO"", ""TIEMPO_INICIADO"") values (:IDC, :IDP, :EST, current_timestamp  at time zone 'America/Lima')";

              using (NpgsqlCommand cmd = new NpgsqlCommand(dml, connection))
              {
                  cmd.Parameters.AddWithValue("IDP", estadoCompetenciaDto.idPostulante);
                  cmd.Parameters.AddWithValue("IDC", estadoCompetenciaDto.idCompetencia);
                  cmd.Parameters.AddWithValue("EST",  !string.IsNullOrEmpty(estadoCompetenciaDto.estado) ? estadoCompetenciaDto.estado.ToUpper() : null);

                  try
                  {
                      var result = cmd.ExecuteNonQuery();
                  }
                  catch (Exception ex)
                  {
                     status = false;
                  }
              }
            }
            return status;
        }

        public async Task<Boolean> actualizarEstadoCompetencia(EstadoCompetenciaDTO estadoCompetenciaDto)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            Boolean status = true;

             if(estadoCompetenciaDto.estado != null){
                if(estadoCompetenciaDto.estado.ToLower() != "i" && estadoCompetenciaDto.estado.ToLower() != "f"){
                 throw new Exception("El estado debe contener solo el valor de I o F");
                }
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
              connection.Open();

              string dml = $@"update estado_competencia set ""ESTADO"" = 'F', ""TIEMPO_FINALIZADO"" = current_timestamp  at time zone 'America/Lima' where ""ID_POSTULANTE"" = {estadoCompetenciaDto.idPostulante} and ""ID_COMPETENCIA"" = {estadoCompetenciaDto.idCompetencia}";

              using (NpgsqlCommand cmd = new NpgsqlCommand(dml, connection))
              {
                  try
                  {
                      var result = cmd.ExecuteNonQuery();
                  }
                  catch (Exception ex)
                  {
                     status = false;
                  }
              }
            }
            return status;
        }

        public async Task<Boolean> alumnoHabilitado(string? dniAlumno)
        {
            string connectionString = _configuration["ConnectionStrings:DefaultConnection"]!;
            Boolean existe = false;

            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand(@"SELECT 1 FROM alumno 
            WHERE dni = @dniAlum AND habilitado_prueba = true LIMIT 1", connection))
            {
                cmd.Parameters.AddWithValue("@dniAlum", dniAlumno);
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
    }
}
