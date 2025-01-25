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
    public string Imagen { get; set; }
    public int Anio { get; set; }
}