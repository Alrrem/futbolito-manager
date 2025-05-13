namespace FutbolitoManager.Models
{
    public class Partido
    {
        public int Id { get; set; }
        public int EquipoLocalId { get; set; }
        public Equipo EquipoLocal { get; set; }
        public int EquipoVisitanteId { get; set; }
        public Equipo EquipoVisitante { get; set; }
        public int GolesLocal { get; set; }
        public int GolesVisitante { get; set; }
        public DateTime Fecha { get; set; }
        public bool Finalizado { get; set; }

        public int CanchaId { get; set; }
        public Cancha Cancha { get; set; }

        public ICollection<PartidoJugador> PartidoJugadores { get; set; }


            = new List<PartidoJugador>();
    }
}
