using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokeMarket.Models
{
    public class CardTag
    {
        // Relacionamos el modelo Card con el modelo Tag
        [Required(ErrorMessage = "El id de la carta es obligatorio.")]
        public int CardId { get; set; }

        [ForeignKey("CardId")]
        public Card Card { get; set; } = null!;

        [Required(ErrorMessage = "El id del tag es obligatorio.")]
        public int TagId { get; set; }
        [ForeignKey("TagId")]
        public Tag Tag { get; set; } = null!;

    }
}
