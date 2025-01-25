using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class PreguntaUpdateDto
    {
        [Required(ErrorMessage = "El valor de Id es requerido")]
        [DefaultValue(1)]
        [Range(1, int.MaxValue, ErrorMessage = "El valor del Id debe ser un número entero positivo.")]
        public int Id { get; set; }

        //[Required(ErrorMessage = "El valor del ProcesoId es requerido")]
        //[DefaultValue(1)]
        //[Range(1, int.MaxValue, ErrorMessage = "El valor del ProcesoId debe ser un número entero positivo.")]
        //public int ProcesoId { get; set; }
        ////public int CompetenciaId { get; set; }

        //[Required(ErrorMessage = "El valor de PreguntaArchivoCargaId es requerido")]
        //[DefaultValue(1)]
        //[Range(1, int.MaxValue, ErrorMessage = "El valor de PreguntaArchivoCargaId debe ser un número entero positivo.")]
        //public int PreguntaArchivoCargaId { get; set; }

        public string TipoEvaluacion { get; set; } = null!;
        public string Grupo { get; set; } = null!;
        public string TextoTitulo { get; set; } = null!;
        public string TextoSuperior { get; set; } = null!;
        public string TextoImagen { get; set; } = null!;
        public string TextoInferior { get; set; } = null!;
        public string TextoPregunta { get; set; } = null!;
        public string OpcionA { get; set; } = null!;
        public string OpcionB { get; set; } = null!;
        public string OpcionC { get; set; } = null!;
        public string OpcionD { get; set; } = null!;
        public string RespuestaCorrecta { get; set; } = null!;
    }
}
