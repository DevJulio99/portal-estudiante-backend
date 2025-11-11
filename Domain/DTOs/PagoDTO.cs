public class PagoDTO
{
    public int IdPago { get; set; }
    public string DocumentoPago { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public string Ciclo { get; set; }
    public decimal Saldo { get; set; }
    public decimal Mora { get; set; }
    public decimal TotalAPagar { get; set; }
    public string Detalle { get; set; }
    public string? Imagen { get; set; }
    public int Anio { get; set; }
    public int total { get; set; }
    public string Estado { get; set; } = "Pendiente"; // Pendiente, En Revisión, Aprobado, Rechazado
    public DateTime? FechaSubidaComprobante { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public int? IdUsuarioAprobador { get; set; }
    public string? Observaciones { get; set; }
    public string? NombreAlumno { get; set; }
}