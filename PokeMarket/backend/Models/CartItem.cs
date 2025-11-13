namespace PokeMarket.Models
{
    public class CartItem
    {
        // Este es el modelo para el carrito en la base de datos
        public int Id { get; set; }

        // Datos del usuario porque va asociado
        public int UserId { get; set; }
        public User User { get; set; } = null!;

		// Datos de la carta
		public int CardId { get; set; }
        public Card Card { get; set; } = null!;

        // Cantidad de producto
        public int Quantity { get; set; } = 1;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;


        
    }
}
