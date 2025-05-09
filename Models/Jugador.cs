using System.ComponentModel.DataAnnotations;

namespace FutbolitoManager.Models
{
    public class Jugador
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Nombre { get; set; }

        [Required, MaxLength(12)]
        public string Rut { get; set; }

        [Required, Range(6, 14)]
        public int Edad { get; set; }

        [Required, StringLength(50)]
        public string Posicion { get; set; }

        // Campo "total de goles" si quieres, o lo puedes eliminar 
        // si vas a llevar estadísticas sólo en PartidoJugador:
        public int Goles { get; set; }

        // FK habitual a Equipo (uno a muchos)
        public int EquipoId { get; set; }
        public virtual Equipo Equipo { get; set; }

        // Relación muchos-a-muchos con Partido —  
        // PartidoJugador será la tabla intermedia
        public virtual ICollection<PartidoJugador> PartidoJugadores { get; set; }
            = new List<PartidoJugador>();
    }
}
