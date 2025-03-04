namespace MyPortalStudent.Domain
{
    public class ResultadoEvaluacionDTO
    {
        public List<ResultadoPreguntaDTO> preguntas { get; set; }
        public int totalPreguntas { get; set; }
        public int correctas { get; set; }
        public int incorrectas { get; set; }
        public int enblanco { get; set; }
        public int puntaje { get; set; }
    }
}