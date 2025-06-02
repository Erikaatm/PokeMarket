namespace PokeMarket.Models.DTOs
{
	public class CartItemResponse
	{
		public int Id { get; set; }

		public int CardId { get; set; }
		public string PokemonName { get; set; } = string.Empty;
		public string ImageUrl { get; set; } = string.Empty;
		public decimal Price { get; set; } 

		public int Quantity { get; set; }


	}
}
