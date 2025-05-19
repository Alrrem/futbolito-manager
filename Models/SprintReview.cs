namespace FutbolitoManager.Models
{
    public class SprintReview
    {
        public int Id { get; set; }
        public int SprintId { get; set; }
        public Sprint Sprint { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public string ComentariosPO { get; set; } = string.Empty;   // feedback del Product Owner
        public bool Aprobado { get; set; }                           // si valida el entregable
    }
}
