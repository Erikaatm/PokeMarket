namespace PokeMarket.Models.DTOs
{
    public class AddTagToCardRequest
    {
        public int CardId { get; set; }
        public int TagId { get; set; }
    }
}
