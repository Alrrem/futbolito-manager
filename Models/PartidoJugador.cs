namespace FutbolitoManager.Models
{
    public class PartidoJugador
    {
        public int Id { get; set; }
        public int PartidoId { get; set; }
        public Partido Partido { get; set; }
        public int JugadorId { get; set; }
        public Jugador Jugador { get; set; }
        public int Goles { get; set; }
    }
}
