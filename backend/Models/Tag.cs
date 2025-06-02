using System.ComponentModel.DataAnnotations;

namespace PokeMarket.Models
{
    public class Tag
    {
        public int Id { get; set; }

        // Nombre del tag
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        // Fecha de creación del tag
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;


        // Relacion con la tabla Card
        public List<CardTag> CardTags { get; set; } = new();

    }
}
