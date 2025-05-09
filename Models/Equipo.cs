namespace FutbolitoManager.Models
{
    public class Equipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Vestimenta { get; set; }
        public string Portero { get; set; }
        public string Capitan { get; set; }

        //conteo de banca 
        public int JugadoresEnBanca { get; set; }

        public string Balon { get; set; }
        public byte[] Logo { get; set; }

        //agrega colección de jugadores:
        public virtual ICollection<Jugador> Jugadores { get; set; }
            = new List<Jugador>();
    }
}
