using System.ComponentModel.DataAnnotations;

namespace FutbolitoManager.Models
{
    public class JugadorFormModel : IValidatableObject
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El RUT es obligatorio")]
        [RegularExpression(@"^[0-9]{7,8}\-[0-9Kk]{1}$", ErrorMessage = "Formato de RUT inválido (ej: 12345678-9)")]
        public string Rut { get; set; }

        [Range(10, 12, ErrorMessage = "La edad debe estar entre 10 y 12 años")]
        public int Edad { get; set; }

        [Required(ErrorMessage = "La posición es obligatoria")]
        public string Posicion { get; set; }

        [Range(0, 999, ErrorMessage = "Goles inválidos")]
        public int Goles { get; set; }

        [Required]
        public int EquipoId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validación extra: Rut “numérico” para descartar menores de ~10M
            var rutSinFormato = Rut.Replace(".", "").Replace("-", "");
            if (int.TryParse(rutSinFormato, out var rutNum))
            {
                if (rutNum < 10000000)
                {
                    yield return new ValidationResult(
                        "El RUT corresponde a una persona mayor de edad y no puede jugar",
                        new[] { nameof(Rut) }
                    );
                }
            }
        }
    }
}
