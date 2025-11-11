namespace MyPortalStudent.Domain.DTOs
{
    public class AprobarPagoDTO
    {
        public int IdPago { get; set; }
        public int IdUsuarioAprobador { get; set; }
        public string Estado { get; set; } // "Aprobado" o "Rechazado"
        public string? Observaciones { get; set; }
    }
}
