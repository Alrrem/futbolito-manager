namespace FutbolitoManager.Models
{
    public class DailyLog
    {
        public int Id { get; set; }

        // Relación opcional con Sprint
        public int? SprintId { get; set; }
        public Sprint? Sprint { get; set; }

        public DateTime Fecha { get; set; }

        public string Ayer { get; set; } = string.Empty;
        public string Hoy { get; set; } = string.Empty;
        public string Bloqueos { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
    }
}
