namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class PreguntaDTO
    {
        public int Id { get; set; }
        public int ProcesoId { get; set; }
        //public int CompetenciaId { get; set; }
        public int PreguntaArchivoCargaId { get; set; }

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

        public bool PostulaActivo { get; set; } = true;
        public DateTime PostulaFechaCreacion { get; set; }
        public string PostulaUsuarioCreacion { get; set; } = null!;
        public DateTime? PostulaFechaActualiza { get; set; }
        public string? PostulaUsuarioActualiza { get; set; } = null!;
    }
}