using System.Collections.Generic;

namespace MyPortalStudent.Domain.DTOs.Notas
{
    public class GestionarNotaDto
    {
        public int IdAlumno { get; set; }
        public int IdCurso { get; set; }
        public int IdPeriodo { get; set; }
        public int? IdSubperiodo { get; set; }
        public List<NotaDetalleDto> NotasInsertar { get; set; } = [];
        public List<NotaDetalleDto> NotasActualizar { get; set; } = [];
    }
}