using System.ComponentModel.DataAnnotations;

namespace FutbolitoManager.Models
{
    public class Cancha
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(200)]
        public string Ubicacion { get; set; }

        // 🌟 Reseña libre para la cancha
        public string Resena { get; set; }

        // 🌟 Imagen de la cancha
        public byte[] Imagen { get; set; }
    }
}
