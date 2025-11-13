namespace PokeMarket.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class Card : IValidatableObject
    {
        // Id único de la carta, primary key
        public int Id { get; set; }

        // Relacion con el usuario
        // Id del usuario perteneciente de la carta
        [JsonIgnore]
        public int UserID { get; set; }

        [JsonIgnore]
        public User? User { get; set; }


        // Nombre del pokemon de la carta
        [Required(ErrorMessage = "El nombre del Pokémon es obligatorio")]
        [MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string PokemonName { get; set; } = string.Empty;

        // El tipo que es el pokemon
        [Required(ErrorMessage = "El tipo del Pokémon es obligatorio")]
        public string PokemonType { get; set; } = string.Empty;

        // Rareza de la carta
        [Required(ErrorMessage = "La rareza de la carta Pokémon es obligatorio")]
        public string Rarity { get; set; } = string.Empty;

        // Imagen Url del pokemon
        [Required(ErrorMessage = "La URL de la imagen es obligatoria")]
        [Url(ErrorMessage = "Debe ser una URL válida")]
        public string ImageUrl {  get; set; } = string.Empty;

        // Coleeción de la carta pokemon
        [Required(ErrorMessage = "La colección es obligatoria")]
        public string Collection {  get; set; } = string.Empty;

        // Si la carta está gradeada o no
        public bool Graded { get; set; } = false;

        // Nota que tiene el gradeo de la carta
        // Ponemos una validación para que la nota no sea mayor que 10
        public int Grade { get; set; }

        // Para saber si la carta esta mandada a gradear
		public bool GradeRequested { get; set; } = false;

		// Para saber el estado del gradeo
		[Required]
		[MaxLength(20)]
		public string GradeStatus { get; set; } = "none";

		// Precio de la carta
		[Range(1, 999999, ErrorMessage = "El precio debe ser mayor que 0")]
        public decimal Price { get; set; }

        // Si la carta está disponible para ser cambiada
        public bool Is_tradeable { get; set; } = false;

        // Fecha de creación
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Fecha de modificación
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;

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

        // Relacion con la tabla Tag
        public List<CardTag> CardTags { get; set; } = new();
    }
}
