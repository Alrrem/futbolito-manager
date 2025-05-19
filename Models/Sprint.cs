namespace FutbolitoManager.Models
{
    public class Sprint
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }
        public ICollection<DailyLog> DailyLogs { get; set; } = new List<DailyLog>();
        // Historias asociadas
        public ICollection<UserStory> UserStories { get; set; } = new List<UserStory>();
    }
}
