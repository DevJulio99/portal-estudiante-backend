using System.ComponentModel.DataAnnotations;

namespace MyPortalStudent.Domain.DTOs.CompetenciasGenerales
{
    public class ListaGruposDTO
    {
        public required string grupo { get; set; }
        public required int numeroPreguntas { get; set; }
    }
}
