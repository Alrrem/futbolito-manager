namespace FutbolitoManager.Models
{
    public class SprintRetrospective
    {
        public int Id { get; set; }
        public int SprintId { get; set; }
        public Sprint Sprint { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public string LoQueFuncionó { get; set; } = string.Empty;
        public string LoQueNoFuncionó { get; set; } = string.Empty;
        public string AccionesMejora { get; set; } = string.Empty;
    }
}
