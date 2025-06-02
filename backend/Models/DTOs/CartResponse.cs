using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PokeMarket.Models.DTOs
{
	public class CartResponse
	{
		public List<CartItemResponse> Items { get; set; } = new();
		public decimal Total { get; set; }
		
	}
}
