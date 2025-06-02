namespace PokeMarket.Models.DTOs
{
	public class GradingResponse
	{
		public int Id { get; set; }

		// Informacion de la carta gradeada
		public int CardId { get; set; }
		public string PokemonName { get; set; } = string.Empty;
		public string Collection { get; set; } = string.Empty;
		public string Rarity { get; set; } = string.Empty;
		public string ImageUrl { get; set; } = string.Empty;
		public decimal Price { get; set; }


		public int Grade { get; set; }

		public string GradedByUsername { get; set; } = string.Empty;

		public DateTime GradedAt { get; set; } 
	}
}
