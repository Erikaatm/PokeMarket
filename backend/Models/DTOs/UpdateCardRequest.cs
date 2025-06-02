using System.ComponentModel.DataAnnotations;

namespace PokeMarket.Models.DTOs
{
    // Esta clase es para poder actualizar los datos de una carta
    public class UpdateCardRequest : IValidatableObject
    {
        public string PokemonName { get; set; } = string.Empty;

        public string PokemonType { get; set; } = string.Empty;

        public string Rarity { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string Collection { get; set; } = string.Empty;

        public bool Graded { get; set; } = false;

        public int Grade { get; set; }

        public decimal Price { get; set; }

        public bool Is_tradeable { get; set; }

        public List<int> TagIds { get; set; } = new();

        // Validación personalizada para el Grade
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Graded && (Grade < 1 || Grade > 10))
            {
                yield return new ValidationResult(
                    "Si la carta está gradeada, la nota debe estar entre 1 y 10.",
                    new[] { nameof(Grade) }
                );
            }
            if (!Graded && Grade != 0)
            {
                yield return new ValidationResult(
                    "Si la carta no está gradeada, la nota debe ser 0.",
                    new[] { nameof(Grade) }
                );
            }
        }
    }

}
