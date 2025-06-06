using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokeMarket.Data;
using PokeMarket.Models;
using PokeMarket.Models.DTOs;
using System.Security.Claims;

namespace PokeMarket.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public CartController(ApplicationDbContext context)
		{
			_context = context;
		}

		// ----------Add-----------

		// Método para añadir cartas al carrito
		[Authorize]
		[HttpPost]
		public async Task<ActionResult> AddToCart(AddToCartRequest request)
		{
			// Comprobamos que se realizan bien las validaciones de los DTOs
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Validamos qeu la cantidad sea minimo 1
			if (request.Quantity < 1)
			{
				return BadRequest(new { message = "La cantidad mínima debe ser 1." });
			}

			// Aquí obtenemos el userID del token, no del body
			var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

			if (getUserId == null)
			{
				return Unauthorized(new
				{
					message = "Debes iniciar sesión para crear una carta."
				});
			}

			int userId = int.Parse(getUserId.Value);

			// Buscamos si existe ese CartItem
			var existsCartItem = await _context.CartItems
				.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.CardId == request.CardId);

			// Verificamos que la carta realmente exista en la base de datos
			var cardExists = await _context.Cards.AnyAsync(c => c.Id == request.CardId);
			if (!cardExists)
			{
				return NotFound(new { message = "La carta no existe." });
			}

			if (existsCartItem != null)
			{
				// Si ya existe sumamos la cnatidad
				existsCartItem.Quantity += request.Quantity;
				existsCartItem.AddedAt = DateTime.UtcNow;

			} else
			{
				// Si no existe, la creamos 
				var cartItem = new CartItem
				{
					UserId = userId,
					CardId = request.CardId,
					Quantity = request.Quantity,
					AddedAt = DateTime.UtcNow
				};

				// Lo añadimos a la base de datos
				_context.CartItems.Add(cartItem);
			}

			// Guardamos los cambios
			await _context.SaveChangesAsync();

			return Ok(new
			{
				message = "Carta añadida al carrito correctamente."
			});
		}


		// ----------GetCart-----------

		// Método que nos muestra el carrito
		[Authorize]
		[HttpGet]
		public async Task<ActionResult> GetCart()
		{
			// Aquí obtenemos el userID del token, no del body
			var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

			if (getUserId == null)
			{
				return Unauthorized(new
				{
					message = "Debes iniciar sesión para crear una carta."
				});
			}

			int userId = int.Parse(getUserId.Value);

			// Guardamos las cartas del carrito en una variable
			var cartItems = await _context.CartItems
				.Include(ci => ci.Card)
				.Where(ci => ci.UserId == userId)
				.ToListAsync();

			// Comprobamos si hay alguna carta
			if (!cartItems.Any())
			{
				return Ok(new
				{
					message = "No tienes cartas en tu carrito.",
					carrito = new List<CartItemResponse>(),
					total = 0
				});
			}

			// Si hay cartas en el carrito
			var carrito = cartItems.Select(MapToCartItemResponse).ToList();

			// Calculamos el totoal
			decimal total = carrito.Sum(ci => ci.Price * ci.Quantity);

			// Devolvemos el carrito
			return Ok(new
			{
				message = "Carrito obtenido correctamente.",
				carrito,
				total
			});
		}


		// ----------Update----------

		// Método para cambiar la cantidad de una carta en el carrito
		[Authorize]
		[HttpPut]
		public async Task<ActionResult> UpdateCartItem(UpdateCartItemRequest request)
		{
			// Comprobamos que se realizan bien las validaciones de los DTOs
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Aquí obtenemos el userID del token, no del body
			var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

			if (getUserId == null)
			{
				return Unauthorized(new
				{
					message = "Debes iniciar sesión para crear una carta."
				});
			}

			int userId = int.Parse(getUserId.Value);

			// Buscamos la carta correspondiente
			var cartItem = await _context.CartItems
				.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.CardId == request.CardId);

			if (cartItem == null)
			{
				return NotFound(new
				{
					message = "La carta no está en tu carrito."
				});
			}

			// Validamos qeu la cantidad sea minimo 1
			if (request.Quantity < 1)
			{
				return BadRequest(new { message = "La cantidad mínima debe ser 1." });
			}

			// Cambiamos la cantidad
			cartItem.Quantity = request.Quantity;
			cartItem.AddedAt = DateTime.UtcNow;

			// Guardamos los cambios
			await _context.SaveChangesAsync();

			return Ok(new
			{
				message = "Cantidad actualizada coreectamente."
			});

		}


		// ----------Delete----------
		[Authorize] 
		[HttpDelete("{cardId}")]
		public async Task<ActionResult> DeleteFromCard(int cardId)
		{
			// Aquí obtenemos el userID del token, no del body
			var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

			if (getUserId == null)
			{
				return Unauthorized(new
				{
					message = "Debes iniciar sesión para crear una carta."
				});
			}

			int userId = int.Parse(getUserId.Value);

			// Buscamos la carta que queremos eliminar de nuestro carrito
			var cartItem = await _context.CartItems
				.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.CardId == cardId);

			if (cartItem == null)
			{
				return NotFound(new
				{
					message = "La carta no se encuentra en el carrito."
				});
			}

			// Eliminamos la carta
			_context.CartItems .Remove(cartItem);

			// Guardamos los cambios
			await _context.SaveChangesAsync();

			return Ok(new
			{
				message = "carta eliminada del carrito correctamente."
			});
		}


		// Método para vaciar el carrito
		[Authorize]
		[HttpDelete("clear")]
		public async Task<ActionResult> ClearCart()
		{
			// Obtenemos el userID del token, no del body
			var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

			if (getUserId == null)
			{
				return Unauthorized(new
				{
					message = "Debes iniciar sesión para crear una carta."
				});
			}

			int userId = int.Parse(getUserId.Value);

			// Buscamos todas las cartas que tiene el usuario en el carrito
			var cartItems = await _context.CartItems
				.Where(ci => ci.UserId == userId)
				.ToListAsync();

			// Eliminamos todas las cartas
			_context.CartItems.RemoveRange(cartItems);

			// Guardamos los cambios
			await _context.SaveChangesAsync();

			return Ok(new
			{
				message = "Carrito vaciado correcatamente."
			});
		}
	
		// ----------Funciones----------

		// Funcion para mapear CartItemResponse
		private CartItemResponse MapToCartItemResponse(CartItem item)
		{
			return new CartItemResponse
			{
				Id = item.Id,
				CardId = item.CardId,
				PokemonName = item.Card?.PokemonName ?? string.Empty,
				ImageUrl = item.Card?.ImageUrl ?? string.Empty,
				Price = item.Card?.Price ?? 0,
				Quantity = item.Quantity
			};
		}

	}
}
