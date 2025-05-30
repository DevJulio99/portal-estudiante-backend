﻿using MyPortalStudent.Domain.DTOs.CompetenciasGenerales;
using MyPortalStudent.Domain.IServices;
using Microsoft.AspNetCore.Mvc;
using MyPortalStudent.Domain;
using MyPortalStudent.Domain.DTOs;

namespace MyPortalStudent.Controllers
{
    [Route("api/v1/competencias-generales")]
    [ApiController]
    public class CompetenciasGeneralesController : ControllerBase
    {

        private readonly ICompetenciasGeneralesService _service;

        private readonly string _controllerName = "competencias-generales";

        public CompetenciasGeneralesController(ICompetenciasGeneralesService service)
        {
            _service = service;
        }

        /// <summary>
        /// Insertar Archivo de Carga de banco de preguntas
        /// </summary>
        /// <remarks>**Description:** Servicio que carga banco de preguntas de forma masiva por medio de una archivo xlsx</remarks>
        /// <param name="CompetenciasGeneralesUploadFileDTO"></param>
        /// <response code="201">Archivo Insertado correctamente</response>
        /// <response code="404">Solicitud incorrecta</response>
        /// <response code="500">Error en el servidor</response>
        // [HttpPost("preguntas/upload")]
        // public async Task<ActionResult> UploadFile([FromForm] CompetenciasGeneralesUploadFileDTO uploadFileDTO)
        // {
        //     if (!ModelState.IsValid) return this.BadRequest(ModelState);
        //     var apiResult = new ApiResult<Object>();
        //     try
        //     {
        //         await _service.UploadFile(uploadFileDTO);
        //         apiResult.Message = string.Format("Archivo de Carga de banco de preguntas subido con éxito", _controllerName);
        //         return this.Created(String.Empty, apiResult);
        //     }
        //     catch (Exception ex)
        //     {
        //         apiResult.Success = false;
        //         apiResult.Message = ex.Message;
        //         return this.StatusCode(500, apiResult);
        //     }
        // }

        [HttpPost("generar-examen")]
        public async Task<ActionResult> generarExamenAleatorio(GenerarExamenDTO request)
        {

            var apiResult = new ApiResult<Object>();

            try
            {
                int status = await _service.ExamenAleatorio(request);
                apiResult.Code = ConstantesPortal.ErrorRequest.code4004;
                apiResult.Success = false;

                if(status == 1){
                    apiResult.Success = true;
                    apiResult.Code = ConstantesPortal.Success.code20018;
                    apiResult.Message = ConstantesPortal.Success.Message20018;
                    return this.Ok(apiResult);
                }
                if(status == 3){
                    apiResult.Code = ConstantesPortal.ErrorRequest.code40010;
                    apiResult.Message = ConstantesPortal.ErrorRequest.Message40010;
                }
                return this.StatusCode(404, apiResult);
               
            }
            catch (Exception ex)
            {
                apiResult.Code = ConstantesPortal.ErrorInterno.code5000;
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }

        [HttpGet("listar-examen/{idPostulante}/{idCompetencia}")]
        public async Task<ActionResult> getExamen(int idPostulante, int idCompetencia)
        {
            var apiResult = new ApiResult<Object>();
            try
            {
                var listaExamen = await _service.listarExamen(idPostulante, idCompetencia);
                apiResult.Message = string.Format(listaExamen.Count.Equals(0) ? "No se encontro datos" : "Se encontro datos", _controllerName);
                apiResult.Success = listaExamen.Count > 0;
                apiResult.Data = listaExamen;
                if(listaExamen.Count == 0){
                    return this.StatusCode(404, apiResult);
                }
                return this.Ok(apiResult);
            }
            catch (Exception ex)
            {
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }

        [HttpGet("competencias")]
        public async Task<ActionResult> getCompetencias(int idPostulante)
        {
            var apiResult = new ApiResult<Object>();
            try
            {
                var listaCompetencia = await _service.listarCompetencias(idPostulante);
                apiResult.Message = string.Format(listaCompetencia.Count.Equals(0) ? "No se encontro datos" : "Se encontro datos", _controllerName);
                apiResult.Data = listaCompetencia;
                apiResult.Success = listaCompetencia.Count > 0;
                if(listaCompetencia.Count == 0){
                    return this.StatusCode(404, apiResult);
                }
                return this.Ok(apiResult);
            }
            catch (Exception ex)
            {
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }

        [HttpGet("competencias-finalizadas")]
        public async Task<ActionResult> getCompetenciasFinalizadas(int idPostulante)
        {
            var apiResult = new ApiResult<Object>();
            try
            {
                var listaCompetencia = await _service.listarCompetenciasFinalizadas(idPostulante);
                apiResult.Success = listaCompetencia.Count > 0;
                apiResult.Message = string.Format(listaCompetencia.Count.Equals(0) ? "No se encontro datos" : "Se encontro datos", _controllerName);
                apiResult.Data = listaCompetencia;
                if(listaCompetencia.Count == 0){
                    return this.StatusCode(404, apiResult);
                }
                return this.Ok(apiResult);
            }
            catch (Exception ex)
            {
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }

         [HttpPost("actualizar-respuesta")]
        public async Task<ActionResult> ActualizarRespuesta(RespuestaReq request)
        {

            var apiResult = new ApiResult<Object>();

            try
            {
                await _service.ActualizarRespuesta(request);
                apiResult.Message = string.Format("Se actualizo la respuesta correctamente", _controllerName);
                return this.Ok(apiResult);
            }
            catch (Exception ex)
            {
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }

        [HttpGet("completada/{idPostulante}/{idCompetencia}")]
        public async Task<ActionResult> CompetenciaCompleta(int idPostulante, int idCompetencia)
        {
            var apiResult = new ApiResult<Object>();

            try
            {
                var completed = await _service.CompetenciaCompleta(idPostulante, idCompetencia);
                apiResult.Message = string.Format((completed ? "Completo" : "No completo") + " la competencia", _controllerName);
                apiResult.Success = completed;
                return this.Ok(apiResult);
            }
            catch (Exception ex)
            {
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }

        [HttpGet("listar-postulante")]
        public async Task<ActionResult> ListarPostulante(string? dniPostulante)
        {
            var apiResult = new ApiResult<Object>();

            try
            {
                var lista = await _service.listarPostulante(dniPostulante);
                apiResult.Message = lista.Count > 0 ? "Se encontro postulante" : "No se pudo encontrar postulante";
                apiResult.Success = lista.Count > 0;
                apiResult.Data = lista;
                if(lista.Count == 0){
                    return this.StatusCode(404, apiResult);
                }
                return this.Ok(apiResult);
            }
            catch (Exception ex)
            {
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }

        [HttpPost("registrar-postulante")]
        public async Task<ActionResult> RegistrarPostulante(RegistrarPostulanteDTO postulanteDto)
        {
            var apiResult = new ApiResult<Object>();

            try
            {
                var success = await _service.registrarPostulante(postulanteDto);
                apiResult.Message = success ? "Se registro postulante" : "No se pudo registrar postulante";
                apiResult.Success = success;
                return this.Ok(apiResult);
            }
            catch (Exception ex)
            {
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }

        [HttpPost("registrar-actividad-postulante")]
        public async Task<ActionResult> RegistrarActividadPostulante(UltimaActividadDTO ultimaActividadDto)
        {
            var apiResult = new ApiResult<Object>();

            try
            {
                var success = await _service.registrarActividadPostulante(ultimaActividadDto);
                apiResult.Message = success ? "Se registro actividad" : "No se pudo registrar actividad";
                apiResult.Success = success;
                return this.Ok(apiResult);
            }
            catch (Exception ex)
            {
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }

        [HttpGet("ultima-actividad-postulante/{idPostulante}/{idCompetencia}")]
        public async Task<ActionResult> UltimaActividadPostulante(int idPostulante, int idCompetencia)
        {
            var apiResult = new ApiResult<Object>();

            try
            {
                var ultimaRespuesta = await _service.getUltimaRespuestaExamen(idPostulante, idCompetencia);
                apiResult.Message = ultimaRespuesta.Count > 0 ? "Se encontro actividad" : "No se pudo encontrar actividad";
                apiResult.Success = ultimaRespuesta.Count > 0;
                apiResult.Data = ultimaRespuesta;
                return this.Ok(apiResult);
            }
            catch (Exception ex)
            {
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }

        [HttpGet("listar-estado-competencia/{idPostulante}/{idCompetencia}")]
        public async Task<ActionResult> RegistrarEstadoCompetencia(int idPostulante, int idCompetencia)
        {
            var apiResult = new ApiResult<Object>();

            try
            {
                var lista = await _service.listarEstadoCompetencia(idPostulante , idCompetencia);
                apiResult.Message = lista.Count > 0 ? "Se encontro estados" : "No se pudo encontrar estados";
                apiResult.Success = lista.Count > 0;
                apiResult.Data = lista;
                if(lista.Count == 0){
                    return this.StatusCode(404, apiResult);
                }
                return this.Ok(apiResult);
            }
            catch (Exception ex)
            {
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }

        [HttpPost("registrar-estado-competencia")]
        public async Task<ActionResult> RegistrarEstadoCompetencia(EstadoCompetenciaDTO estadoCompetenciaDto)
        {
            var apiResult = new ApiResult<Object>();

            try
            {
                var success = await _service.registrarEstadoCompetencia(estadoCompetenciaDto);
                apiResult.Message = success ? "Se registro estado" : "No se pudo registrar estado";
                apiResult.Success = success;
                return this.Ok(apiResult);
            }
            catch (Exception ex)
            {
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }

        [HttpPut("actualizar-estado-competencia")]
        public async Task<ActionResult> ActualizarEstadoCompetencia(EstadoCompetenciaDTO estadoCompetenciaDto)
        {
            var apiResult = new ApiResult<Object>();

            try
            {
                var success = await _service.actualizarEstadoCompetencia(estadoCompetenciaDto);
                apiResult.Message = success ? "Se actualizo estado" : "No se pudo actualizar estado";
                apiResult.Success = success;
                return this.Ok(apiResult);
            }
            catch (Exception ex)
            {
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }

        [HttpGet("alumno-habilitado")]
        public async Task<ActionResult> AlumnoHabilitado(string dniAlumno)
        {
            var apiResult = new ApiResult<Object>();

            try
            {
                var success = await _service.alumnoHabilitado(dniAlumno);
                apiResult.Message = success ? "El alumno esta habilitado" : "El alumno no esta habilitado";
                apiResult.Success = success;
                return this.Ok(apiResult);
            }
            catch (Exception ex)
            {
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }

        [HttpGet("resultado-competencia")]
        public async Task<ActionResult> resultadoCompetencia(int idPostulante, int idCompetencia)
        {
            var apiResult = new ApiResult<Object>();

            try
            {
                var lista = await _service.resultadoCompetencia(idPostulante, idCompetencia);
                apiResult.Message = lista.Count > 0 ? "Se encontraron resultados." : "No se encontraron resultados.";
                apiResult.Success = lista.Count > 0;
                apiResult.Data = lista;
                if(lista.Count == 0){
                    return this.StatusCode(404, apiResult);
                }
                return this.Ok(apiResult);
            }
            catch (Exception ex)
            {
                apiResult.Success = false;
                apiResult.Message = ex.Message;
                return this.StatusCode(500, apiResult);
            }
        }
    }
}
