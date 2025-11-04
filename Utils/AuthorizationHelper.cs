using System.Security.Claims;
using System;

namespace MyPortalStudent.Utils
{
    /// <summary>
    /// Helper para validar autorización de usuarios
    /// </summary>
    public static class AuthorizationHelper
    {
        /// <summary>
        /// Obtiene el ID del alumno del token JWT
        /// </summary>
        public static int? GetAlumnoIdFromClaims(ClaimsPrincipal? user)
        {
            if (user == null) return null;
            
            var idAlumnoClaim = user.FindFirstValue("Id_Alumno");
            if (string.IsNullOrWhiteSpace(idAlumnoClaim) || !int.TryParse(idAlumnoClaim, out var idAlumno))
                return null;
                
            return idAlumno;
        }

        /// <summary>
        /// Obtiene el código de sede del token JWT
        /// </summary>
        public static string? GetCodigoSedeFromClaims(ClaimsPrincipal? user)
        {
            return user?.FindFirstValue("Codigo_Sede");
        }

        /// <summary>
        /// Obtiene el rol del usuario del token JWT
        /// </summary>
        public static string? GetRoleFromClaims(ClaimsPrincipal? user)
        {
            return user?.FindFirstValue("Role");
        }

        /// <summary>
        /// Valida si un usuario puede acceder a datos de un alumno específico
        /// Un usuario puede ver:
        /// - Sus propios datos (si Id_Alumno coincide)
        /// - Datos de otros alumnos si es Admin o Docente (validado por rol)
        /// </summary>
        public static bool CanAccessAlumnoData(ClaimsPrincipal? user, int idAlumnoRequested)
        {
            if (user == null) return false;

            var idAlumno = GetAlumnoIdFromClaims(user);
            var role = GetRoleFromClaims(user);

            // Si el usuario está consultando sus propios datos, permitir
            if (idAlumno.HasValue && idAlumno.Value == idAlumnoRequested)
                return true;

            // Si es Admin o Docente, puede ver datos de otros alumnos (pero RLS los filtrará por tenant)
            if (!string.IsNullOrWhiteSpace(role) && 
                (role.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
                 role.Equals("Docente", StringComparison.OrdinalIgnoreCase)))
                return true;

            return false;
        }

        /// <summary>
        /// Valida si un usuario puede acceder a datos por DNI
        /// Similar a CanAccessAlumnoData pero para búsquedas por DNI
        /// </summary>
        public static bool CanAccessAlumnoByDni(ClaimsPrincipal? user, string? dniRequested)
        {
            if (user == null || string.IsNullOrWhiteSpace(dniRequested)) return false;

            var dniUsuario = user.FindFirstValue("Dni_Usuario");
            var role = GetRoleFromClaims(user);

            // Si el usuario está consultando sus propios datos, permitir
            if (!string.IsNullOrWhiteSpace(dniUsuario) && 
                dniUsuario.Equals(dniRequested, StringComparison.OrdinalIgnoreCase))
                return true;

            // Si es Admin o Docente, puede ver datos de otros alumnos (pero RLS los filtrará por tenant)
            if (!string.IsNullOrWhiteSpace(role) && 
                (role.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
                 role.Equals("Docente", StringComparison.OrdinalIgnoreCase)))
                return true;

            return false;
        }
    }
}
