namespace MyPortalStudent.Domain.DTOs
{
    /// <summary>
    /// DTO para actualizar el estado de una matrícula
    /// </summary>
    public class ActualizarEstadoMatriculaDTO
    {
        /// <summary>
        /// Nuevo estado de la matrícula (Activa, Inactiva)
        /// </summary>
        public string NuevoEstado { get; set; } = string.Empty;
    }
}
