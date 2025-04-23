namespace MyPortalStudent.Domain.DTOs
{
    public class ConstantesPortal
    {
        public struct Success
        {
            public const string code2000 = "PS-EVAL-2000";
            public const string Message2000 = "Se logueó correctamente";
             public const string code2001 = "PS-EVAL-2001";
            public const string Message2001 = "Se obtuvo la información correctamente";

            public const string code2002 = "PS-EVAL-2002";
            public const string Message2002 = "Se encontraron usuarios";

            public const string code2008 = "PS-EVAL-2008";
            public const string Message2008 = "Se obtuvo acceso";

            public const string code20010 = "PS-EVAL-20010";
            public const string Message20010 = "Se modificó la contraseña";

            public const string code20012 = "PS-EVAL-20012";
            public const string Message20012 = "Se envió código de verificación";

            public const string code20014 = "PS-EVAL-20014";
            public const string Message20014 = "Se validó el usuario";

            public const string code20015 = "PS-EVAL-20015";
            public const string Message20015 = "Se generó el captcha";

            public const string code20016 = "PS-EVAL-20016";
            public const string Message20016 = "Se validó el captcha";

            public const string code20017 = "PS-EVAL-20017";
            public const string Message20017 = "Se cerró sesión correctamente";
        }

        public struct ErrorRequest
        {
            public static string code4000 { get; } = "PS-EVAL-4000";
            public static string Message4000 { get; } = "No se ha ingresado los valores requeridos.";
            public static string code4001 { get; } = "PS-EVAL-4001";
            public static string Message4001 { get; } = "CAPTCHA no válido.";
            public static string code4002 { get; } = "PS-EVAL-4002";
            public static string Message4002 { get; } = "El código CAPTCHA ha expirado.";
            public static string code4004 { get; } = "PS-EVAL-4004";
            public static string Message4004 { get; } = "No se encontró datos";
            public static string code4005 { get; } = "PS-EVAL-4005";
            public static string Message4005 { get; } = "Unauthorized. La sesión ha expirado o es inválida, por favor inicie sesión otra vez.";
            public static string code4006 { get; } = "PS-EVAL-4006";
            public static string Message4006 { get; } = "Permiso Denegado. El usuario no tiene acceso a esta información.";
            public static string code4009 { get; } = "PS-EVAL-4009";
            public static string Message4009 { get; } = "Conflicto";

            
        }

        public struct ErrorInterno
        {
            public static string code5000 { get; } = "PS-EVAL-5000";
            public static string Message5000 { get; } = "Respuesta inesperada del servicio.";
        }
    }
}
