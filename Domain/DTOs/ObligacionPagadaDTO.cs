public class ObligacionPagadaDTO
{
    public string Periodo { get; set; }
    public string FechaPago { get; set; }
    public string Concepto { get; set; }
    public string NumeroDocumentoPago { get; set; }
    public int NumeroCuota { get; set; }
    public decimal Importe { get; set; }
    public decimal MontoPagado { get; set; }
}

public class ObligacionPorPeriodoDTO
{
    public string Periodo { get; set; }
    public List<ObligacionPagadaDTO> Pagos { get; set; }
}
