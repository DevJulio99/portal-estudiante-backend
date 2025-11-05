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
            int status = await _service.ExamenAleatorio(request);
            apiResult.Code = ConstantesPortal.ErrorRequest.code4004;
            apiResult.Success = false;

            if (status == 1)
            {
                apiResult.Success = true;
                apiResult.Code = ConstantesPortal.Success.code20018;
                apiResult.Message = ConstantesPortal.Success.Message20018;
                return this.Ok(apiResult);
            }
            if (status == 3)
            {
                apiResult.Code = ConstantesPortal.ErrorRequest.code40010;
                apiResult.Message = ConstantesPortal.ErrorRequest.Message40010;
            }
            return this.StatusCode(404, apiResult);
        }

        [HttpGet("listar-examen/{idPostulante}/{idCompetencia}")]
        public async Task<ActionResult> getExamen(int idPostulante, int idCompetencia)
        {
            var apiResult = new ApiResult<Object>();
            var listaExamen = await _service.listarExamen(idPostulante, idCompetencia);
            apiResult.Message = string.Format(listaExamen.Count.Equals(0) ? "No se encontró datos" : "Se encontró datos", _controllerName);
            apiResult.Success = listaExamen.Count > 0;
            apiResult.Data = listaExamen;
            if (listaExamen.Count == 0)
            {
                return this.StatusCode(404, apiResult);
            }
            return this.Ok(apiResult);
        }

        [HttpGet("competencias")]
        public async Task<ActionResult> getCompetencias(int idPostulante)
        {
            var apiResult = new ApiResult<Object>();
            var listaCompetencia = await _service.listarCompetencias(idPostulante);
            apiResult.Message = string.Format(listaCompetencia.Count.Equals(0) ? "No se encontró datos" : "Se encontró datos", _controllerName);
            apiResult.Data = listaCompetencia;
            apiResult.Success = listaCompetencia.Count > 0;
            if (listaCompetencia.Count == 0)
            {
                return this.StatusCode(404, apiResult);
            }
            return this.Ok(apiResult);
        }

        [HttpGet("competencias-finalizadas")]
        public async Task<ActionResult> getCompetenciasFinalizadas(int idPostulante)
        {
            var apiResult = new ApiResult<Object>();
            var listaCompetencia = await _service.listarCompetenciasFinalizadas(idPostulante);
            apiResult.Success = listaCompetencia.Count > 0;
            apiResult.Message = string.Format(listaCompetencia.Count.Equals(0) ? "No se encontró datos" : "Se encontró datos", _controllerName);
            apiResult.Data = listaCompetencia;
            if (listaCompetencia.Count == 0)
            {
                return this.StatusCode(404, apiResult);
            }
            return this.Ok(apiResult);
        }

         [HttpPost("actualizar-respuesta")]
        public async Task<ActionResult> ActualizarRespuesta(RespuestaReq request)
        {

            var apiResult = new ApiResult<Object>();
            await _service.ActualizarRespuesta(request);
            apiResult.Message = string.Format("Se actualizó la respuesta correctamente", _controllerName);
            return this.Ok(apiResult);
        }

        [HttpGet("completada/{idPostulante}/{idCompetencia}")]
        public async Task<ActionResult> CompetenciaCompleta(int idPostulante, int idCompetencia)
        {
            var apiResult = new ApiResult<Object>();
            var completed = await _service.CompetenciaCompleta(idPostulante, idCompetencia);
            apiResult.Message = string.Format((completed ? "Completó" : "No completó") + " la competencia", _controllerName);
            apiResult.Success = completed;
            return this.Ok(apiResult);
        }

        [HttpGet("listar-postulante")]
        public async Task<ActionResult> ListarPostulante(string? dniPostulante)
        {
            var apiResult = new ApiResult<Object>();
            var lista = await _service.listarPostulante(dniPostulante);
            apiResult.Message = lista.Count > 0 ? "Se encontró postulante" : "No se pudo encontrar postulante";
            apiResult.Success = lista.Count > 0;
            apiResult.Data = lista;
            if (lista.Count == 0)
            {
                return this.StatusCode(404, apiResult);
            }
            return this.Ok(apiResult);
        }

        [HttpPost("registrar-postulante")]
        public async Task<ActionResult> RegistrarPostulante(RegistrarPostulanteDTO postulanteDto)
        {
            var apiResult = new ApiResult<Object>();
            var success = await _service.registrarPostulante(postulanteDto);
            apiResult.Message = success ? "Se registró postulante" : "No se pudo registrar postulante";
            apiResult.Success = success;
            return this.Ok(apiResult);
        }

        [HttpPost("registrar-actividad-postulante")]
        public async Task<ActionResult> RegistrarActividadPostulante(UltimaActividadDTO ultimaActividadDto)
        {
            var apiResult = new ApiResult<Object>();
            var success = await _service.registrarActividadPostulante(ultimaActividadDto);
            apiResult.Message = success ? "Se registró actividad" : "No se pudo registrar actividad";
            apiResult.Success = success;
            return this.Ok(apiResult);
        }

        [HttpGet("ultima-actividad-postulante/{idPostulante}/{idCompetencia}")]
        public async Task<ActionResult> UltimaActividadPostulante(int idPostulante, int idCompetencia)
        {
            var apiResult = new ApiResult<Object>();
            var ultimaRespuesta = await _service.getUltimaRespuestaExamen(idPostulante, idCompetencia);
            apiResult.Message = ultimaRespuesta.Count > 0 ? "Se encontró actividad" : "No se pudo encontrar actividad";
            apiResult.Success = ultimaRespuesta.Count > 0;
            apiResult.Data = ultimaRespuesta;
            return this.Ok(apiResult);
        }

        [HttpGet("listar-estado-competencia/{idPostulante}/{idCompetencia}")]
        public async Task<ActionResult> RegistrarEstadoCompetencia(int idPostulante, int idCompetencia)
        {
            var apiResult = new ApiResult<Object>();
            var lista = await _service.listarEstadoCompetencia(idPostulante, idCompetencia);
            apiResult.Message = lista.Count > 0 ? "Se encontró estados" : "No se pudo encontrar estados";
            apiResult.Success = lista.Count > 0;
            apiResult.Data = lista;
            if (lista.Count == 0)
            {
                return this.StatusCode(404, apiResult);
            }
            return this.Ok(apiResult);
        }

        [HttpPost("registrar-estado-competencia")]
        public async Task<ActionResult> RegistrarEstadoCompetencia(EstadoCompetenciaDTO estadoCompetenciaDto)
        {
            var apiResult = new ApiResult<Object>();
            var success = await _service.registrarEstadoCompetencia(estadoCompetenciaDto);
            apiResult.Message = success ? "Se registró estado" : "No se pudo registrar estado";
            apiResult.Success = success;
            return this.Ok(apiResult);
        }

        [HttpPut("actualizar-estado-competencia")]
        public async Task<ActionResult> ActualizarEstadoCompetencia(EstadoCompetenciaDTO estadoCompetenciaDto)
        {
            var apiResult = new ApiResult<Object>();
            var success = await _service.actualizarEstadoCompetencia(estadoCompetenciaDto);
            apiResult.Message = success ? "Se actualizó estado" : "No se pudo actualizar estado";
            apiResult.Success = success;
            return this.Ok(apiResult);
        }

        [HttpGet("alumno-habilitado")]
        public async Task<ActionResult> AlumnoHabilitado(string dniAlumno)
        {
            var apiResult = new ApiResult<Object>();
            var success = await _service.alumnoHabilitado(dniAlumno);
            apiResult.Message = success ? "El alumno está habilitado" : "El alumno no está habilitado";
            apiResult.Success = success;
            return this.Ok(apiResult);
        }

        [HttpGet("resultado-competencia")]
        public async Task<ActionResult> resultadoCompetencia(int idPostulante, int idCompetencia)
        {
            var apiResult = new ApiResult<Object>();
            var lista = await _service.resultadoCompetencia(idPostulante, idCompetencia);
            apiResult.Message = lista.Count > 0 ? "Se encontraron resultados." : "No se encontraron resultados.";
            apiResult.Success = lista.Count > 0;
            apiResult.Data = lista;
            if (lista.Count == 0)
            {
                return this.StatusCode(404, apiResult);
            }
            return this.Ok(apiResult);
        }
    }
}
