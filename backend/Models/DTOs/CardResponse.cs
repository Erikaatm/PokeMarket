namespace PokeMarket.Models.DTOs
{
    public class CardResponse
    {
        // Información de la carta
        public int Id { get; set; }

        public string PokemonName { get; set; } = string.Empty;

        public string PokemonType { get; set;} = string.Empty;

        public string Rarity { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string Collection { get; set; } = string.Empty;

        public bool Graded { get; set; } = false;

        public int Grade { get; set; }

        public decimal Price { get; set; }  

        public bool Is_tradeable { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }


        // Información del usuario
        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;


        // Tags que tiene la carta asociados
        public List<TagResponse> Tags { get; set; } = new();
        
    }
}
