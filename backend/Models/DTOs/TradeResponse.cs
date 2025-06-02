namespace PokeMarket.Models.DTOs
{
	public class TradeResponse
	{
		public int Id { get; set; }

		// Informacion del usuario que propone el intercambio
		public int RequesterId { get; set; }
		public string RequesterUsername { get; set; } = string.Empty;

		// Información del usuario que recibe el intercambio
		public int ReceiverId { get; set; } 
		public string ReceiverUsername { get; set; } = string.Empty;

		// Carta ofrecida
		public CardResponse OfferedCard { get; set; } = null!;

		// Carta solicitada
		public CardResponse RequestedCard { get; set; } = null!;

		// Estado del intercambio
		public string Status { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; }
		public DateTime LastUpdatedAt { get; set; }


	}
}
