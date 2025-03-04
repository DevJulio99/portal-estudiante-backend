namespace MyPortalStudent.Domain
{
    public class ResultadoCompetenciaDTO
    {
        public int idCompetencia { get; set; }
        public string nombreCompetencia { get; set; }
        public string descripcion { get; set; }
        public Decimal puntajeMinimoAprobatorio { get; set; }
        public Decimal puntajePorPregunta { get; set; }
        public Decimal peso { get; set; }
    }
}