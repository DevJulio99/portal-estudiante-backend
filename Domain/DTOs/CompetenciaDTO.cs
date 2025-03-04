namespace MyPortalStudent.Domain
{
    public class CompetenciaDTO : BaseCompetenciaDTO
    {
        public string pesoCompetencia { get; set; }
        public int numeroPreguntas { get; set;}
        public string fechaDisponibilidad { get; set; }
        public string fechaInicio { get; set; }
        public string fechaFin { get; set; }
        public string horaInicio { get; set; }
        public string ordenCompetencia { get; set; }
        public string estadoCompetencia { get; set; }
        public string dependenciaCompetencia { get; set; }
        public string idEtapa { get; set; }
        public string idProceso { get; set; }
        public string tiempoLimite { get; set; }
        public string urlImagen { get; set; }
        public Boolean finalizado { get; set; }
    }
}
