public class EventoDTO
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string ImagenDesktop { get; set; }
    public string ImagenMobile { get; set; }
    public string AltImagenDesktop { get; set; }
    public string AltImagenMobile { get; set; }
    public string Url { get; set; }
    public string Prioridad { get; set; }
    public bool AbrirNuevaPagina { get; set; }
    public string TipoDeEvento { get; set; }
    public string CategoriaEvento { get; set; }
    public string FechaDeInicio { get; set; }
    public string HoraDeInicio { get; set; }
    public string FechaDeFin { get; set; }
    public string HoraDeFin { get; set; }
    public string FechaInicioEvento { get; set; }
    public string HoraInicioEvento { get; set; }
    public string FechaFinEvento { get; set; }
    public string NombreBoton { get; set; }
    public string Capacidad { get; set; }
    public string Descripcion { get; set; }
    public List<UbicacionEventoDTO> Ubicacion { get; set; }
}