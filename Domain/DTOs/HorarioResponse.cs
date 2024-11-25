namespace MyPortalStudent.Domain
{
    public class HorarioResponse
    {
        public required int idMatricula { get; set; }
        public required HorarioDTO horario { get; set; }
        public required DetalleHorarioDTO[] detalleHorario { get; set; }
    }
}
