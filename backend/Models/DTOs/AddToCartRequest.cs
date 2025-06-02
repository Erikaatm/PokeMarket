namespace PokeMarket.Models.DTOs
{
	public class AddToCartRequest
	{
		public int CardId { get; set; }
		public int Quantity { get; set; } = 1;
	}
}
