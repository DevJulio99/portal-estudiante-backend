namespace MyPortalStudent.Domain {
public class AlumnoDTO
{
    public required int id_alumno { get; set; }
    public required string codigoAlumno { get; set; }
    public required string nombre { get; set; }
    public required string apellidoPaterno { get; set; }
    public required string apellidoMaterno { get; set; }
    public required string dni { get; set; }
    public required string correo { get; set; }
    public required string fechaNacimiento { get; set; }
    public required string telefono { get; set; }
    public required string direccion { get; set; }
    public required string fotoPerfil { get; set; }
    public required string genero { get; set; }
    public required string tipoAlumno { get; set; }
    public required string observaciones { get; set; }
    public required string apoderado { get; set; }
}
}
