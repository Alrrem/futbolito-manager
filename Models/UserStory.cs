namespace FutbolitoManager.Models
{
    public class UserStory
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public StoryStatus Status { get; set; } = StoryStatus.ToDo;
        public int StoryPoints { get; set; }


        public int? SprintId { get; set; }
        public Sprint? Sprint { get; set; }
    }

    public enum StoryStatus
    {
        ToDo,
        InProgress,
        Done
    }
}
