using System.ComponentModel.DataAnnotations;

namespace PokeMarket.Models.DTOs
{
    public class CreateCardRequest : IValidatableObject
    {
        [Required(ErrorMessage = "El nombre del Pokémon es obligatorio.")]
        [MaxLength(50)] 
        public string PokemonName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de Pokémon es olbigatorio.")]
        public string PokemonType { get; set; } = string.Empty;

        [Required(ErrorMessage = "La rareza de la carta Pokémon es obligatorio")]
        public string Rarity { get; set; } = string.Empty;

        [Required(ErrorMessage = "La URL de la imagen es obligatoria.")]
        [Url(ErrorMessage = "Debe ser una URL válida.")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "La colección es obligatoria.")]
        public string Collection {  get; set; } = string.Empty;

        public bool Graded { get; set; } = false;

        public int Grade {  get; set; }


        [Range(1, 999999, ErrorMessage = "El precio debe ser mayor que 0.")]
        public decimal Price { get; set; }

        public bool Is_tradeable { get; set; } = false;

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
