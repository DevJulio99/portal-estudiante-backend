namespace MyPortalStudent.Domain {
public class AlumnoRegistrarDTO
{
    public required string correo { get; set; }
    public string? contraseña { get; set; }
    public required string nombreUsuario { get; set; }
    public required string apellidoPaterno { get; set; }
    public required string apellidoMaterno { get; set; }
    public required string telefono { get; set; }
    public required string numeroDocumento { get; set; }
    public string? codigoSede { get; set; }
    public required string fechaNacimiento { get; set; }
    public required string direccion { get; set; }
    public required string fotoPerfil { get; set; }
    public required string genero { get; set; }
    public required string tipoAlumno { get; set; }
    public required string observaciones { get; set; }
    public required string apoderado { get; set; }
    public required string tipoInstitucion { get; set; }
}
}
