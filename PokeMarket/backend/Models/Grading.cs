using System.ComponentModel.DataAnnotations;

namespace PokeMarket.Models
{
    public class Grading
    {
        // ID único del gradeo (PK)
        public int Id { get; set; }

        // ID de la carta que ha sido gradead (FK)
        [Required(ErrorMessage = "El id de la carta es obligatorio.")]
        public int CardId { get; set; }

        // Navegación a la carta gradeada 
        public Card Card { get; set; } = null!;

        // Nota del gradeo (Entre 1 y 10)
        [Range(1, 10, ErrorMessage = "El gradeo debe estar entre 1 y 10.")]
        public int Grade { get; set; }

        // Usuario que ha hecho el gradeo (debe ser admin) (FK)
        [Required]
        public int GradedById { get; set; }

        // Navegación al usuario que ha gradeado la carta
        public User GradedBy { get; set; } = null!;

        // Fecha en la que se hizo el gradeo 
        public DateTime GradedAt { get; set; } = DateTime.Now;

    }
}
