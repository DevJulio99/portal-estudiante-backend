using System.Collections.Generic;

namespace MyPortalStudent.Domain.DTOs.Notas
{
    public class RegistrarNotaDto
    {
        public int IdAlumno { get; set; }
        public int IdCurso { get; set; }
        public int IdPeriodo { get; set; }
        public int? IdSubperiodo { get; set; }
        public List<NotaDetalleDto> Notas { get; set; } = [];
    }
}