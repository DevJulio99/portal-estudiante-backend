using System.ComponentModel.DataAnnotations;

namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class PreguntaDto
    {
        public required string tipoEvaluacion { get; set; }
        public required string grupo { get; set; }
        public required string textoTitulo { get; set; }
        public required string textoSuperior { get; set; }
        public required string textoImagen { get; set; }
        public required string textoInferior { get; set; }
        public required string textoGrupo { get; set; }
        public int idArchivoCarga { get; set; }
        public required string pregunta { get; set; }
        public required string opcionA { get; set; }
        public required string opcionB { get; set; }
        public required string opcionC { get; set; }
        public required string opcionD { get; set; }
        public required string respuestaCorrecta { get; set; }
        public required int numeroPregunta {get; set;}
        public int idCompetencia { get; set; }
    }
}
