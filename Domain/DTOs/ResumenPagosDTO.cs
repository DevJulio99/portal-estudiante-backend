public class ResumenPagosDTO
{
    public int IdAlumno { get; set; }
    public int PagosVencidos { get; set; }
    public int PagosPorVencer { get; set; }
    public int PagosATiempo { get; set; }
    public decimal MontoTotalPendiente { get; set; }
}
