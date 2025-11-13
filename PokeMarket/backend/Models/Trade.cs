using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PokeMarket.Models
{
    public class Trade
    {
        public int Id { get; set; }

        // El usuario propone un intercambio
        [Required]
        public int RequesterId { get; set; }
        public User Requester { get; set; } = null!;

        // Usuario que recibe la solicitud
        [Required]
        public int ReceiverId { get; set; }
        public User Receiver { get; set; } = null!;

        // Carta ofrecida
        [Required]
        public int OfferedCardId { get; set; }
        public Card OfferedCard { get; set; } = null!;

        // Carta que se quiere conseguir
        [Required]
        public int RequestedCardId { get; set; }
        public Card RequestedCard { get; set; } = null!;

        // Estado del intercambio
        public enum TradeStatus { Pending, Accepted, Rejected}

        [Required]
        // Esto sirve para que en la base de datos aparezca Pending en vez de 0, igual en todos los demas
        [EnumDataType(typeof(TradeStatus))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TradeStatus Status { get; set; } = TradeStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;
    }
}
