namespace PokeMarket.Models.DTOs
{
    public class CreateTradeRequest
    {
        public int OfferCardId { get; set; }
        public int RequestedCardId { get; set; }
    }
}
