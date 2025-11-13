using System.ComponentModel.DataAnnotations;

namespace PokeMarket.Models
{
    public class Favorite
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "El id del usuario es obligatorio.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "El id de la carta es obligatorio.")]
        public int CardId { get; set; }

        public Card Card { get; set; } = null!;


        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}
