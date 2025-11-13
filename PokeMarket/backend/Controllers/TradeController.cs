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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TradeController : ControllerBase
    {
        readonly ApplicationDbContext _context;

        public TradeController (ApplicationDbContext context)
        {
            _context = context;
        }

		// ----------Create----------

		// Método para crear un nuevo trade
		[HttpPost]
        public async Task<ActionResult> CreateTrade(CreateTradeRequest request)
        {
            // Obtenemos el usuario logeado
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if (getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para crear un intercambio."
                });
            }

            int requesterId = int.Parse(getUserId.Value);

            // Verificamos si la carta ofrecida existe y pertenece al usuario logeado
            var offeredCard = await _context.Cards
                .FirstOrDefaultAsync(c => c.Id == request.OfferCardId && c.UserID == requesterId);

            if (offeredCard == null)
            {
                return NotFound(new
                {
                    message = "La carta que se ofrece no existe o no te pertenece."
                });
            }

            // Verificamos si la carta solicitada existe
            var requestedCard = await _context.Cards
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == request.RequestedCardId);

            if (requestedCard == null)
            {
                return NotFound(new
                {
                    message = "Carta no encontrada."
                });
            }

            // No se puede hacer un trade contigo mismo
            if(requestedCard.UserID == requesterId)
            {
                return BadRequest(new
                {
                    message = "No puedes hacer un intercambio con la misma persona que lo solicita."
                });
            }

            // Ahora creamos el trade
            var trade = new Trade
            {
                RequesterId = requesterId,
                ReceiverId = requestedCard.UserID,
                OfferedCardId = offeredCard.Id,
                RequestedCard = requestedCard,
                Status = Trade.TradeStatus.Pending,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now,
            };

            // Guardamos en la base de datos
            _context.Add(trade);
            await _context.SaveChangesAsync();

            // Devolvemos mensaje de exito
            return Ok(new
            {
                message = "Intercambio creado correctamente.",
                trade = trade
            });

        }


		// ----------GetAllSent----------

		// Método para obtener todos los intercambios enviados de un usuario logeado
		[HttpGet("my-trades/sent")]
        public async Task<ActionResult> GetMySentTrades(
            int page = 1,
            string? status = null,
            string? orderBy = null
            )
        {
            if (page < 1) page = 1;

            int pageSize = 10;

            // Obtenemos el id del usuariologeado
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if (getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para ver tus intercambios enviados."
                });
            }

            int userId = int.Parse(getUserId.Value);

            // Obtenemos todos los intercambios que el usuario ha enviado
            var query = _context.Trades
                .Where(t => t.RequesterId == userId)
                .Include(t => t.Requester)
                .Include(t => t.Receiver)
                .Include(t => t.OfferedCard)
                    .ThenInclude(c => c.User)
                .Include(t => t.OfferedCard)
                    .ThenInclude(c => c.CardTags)
                        .ThenInclude(ct => ct.Tag)
				.Include(t => t.RequestedCard)
					.ThenInclude(c => c.User)
                .Include(t => t.RequestedCard)
				    .ThenInclude(c => c.CardTags)
					    .ThenInclude(ct => ct.Tag)
				.AsQueryable();

            // Aplicamos filtros y ordenacion
            query = ApplyTradeStatusFilter(query, status);
            query = ApplyTradeOrdering(query, orderBy);

            // Comprobamos si esta vacío
            if (!await query.AnyAsync())
            {
                return NotFound(new
                {
                    message = "No se ha encontrado ningun trade con esos filtros"
                });
            }

            // Aplicamos la paginacion
            var result = await ApplyPagination(query, page, pageSize);

            return Ok(result);
        }


		// ----------GetAllReceived----------

		// Método para obtener todos los intercambios recibidos de un usuario logeado
		[HttpGet("my-trades/received")]
        public async Task<ActionResult> GetMyReceivedTrades(
            int page = 1,
            string? status = null,
            string? orderBy = null
            )
        {
            if ( page < 1 ) page = 1;

            int pageSize = 10;

            // Obtenemos el id del usuario logeado
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if (getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para ver tus intercambios recibidos."
                });
            }

            int userId = int.Parse(getUserId.Value);

            // Obtenemos todos los intercambios que el usuario ha enviado
            var query = _context.Trades
                .Where(t => t.ReceiverId == userId)
				.Include(t => t.Requester)
				.Include(t => t.Receiver)
				.Include(t => t.OfferedCard)
					.ThenInclude(c => c.User)
				.Include(t => t.OfferedCard)
					.ThenInclude(c => c.CardTags)
						.ThenInclude(ct => ct.Tag)
				.Include(t => t.RequestedCard)
					.ThenInclude(c => c.User)
				.Include(t => t.RequestedCard)
					.ThenInclude(c => c.CardTags)
						.ThenInclude(ct => ct.Tag)
				.AsQueryable();

            // Aplicamos filtros y orden y comprobamos si esta vacío
            query = ApplyTradeStatusFilter(query, status);
            query = ApplyTradeOrdering(query, orderBy);

            if (!await query.AnyAsync())
            {
                return NotFound(new
                {
                    message = "No se ha encontrado ningun trade con esos filtros."
                });
            }

            // Aplicamos el paginado
            var result = ApplyPagination(query, page, pageSize);

            return Ok(result);
        }


		// ----------Accept----------

		// Método para aceptar un trade 
		[HttpPut("accept/{idTrade}")]
        public async Task<ActionResult> AcceptTrade(int idTrade)
        {
            // Verificamos si el usuario esta logeado
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if(getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para aceptar un intercambio."
                });
            }

            int userId = int.Parse(getUserId.Value);

            // Buscamos el Trade
            var trade = await _context.Trades.FindAsync(idTrade);

            if (trade == null)
            {
                return NotFound(new
                {
                    message = "Intercambio no encontrado."
                });
            }

            // Comprobamos que el usuario logeado es el receptor del intercambio
            if(trade.ReceiverId != userId)
            {
                return Unauthorized(new
                {
                    message = "No puedes aceptar un intercambio que no es tuyo."
                });
            }

            // Solo se puede aceptar si el intercambio está pendiente
            if(trade.Status != Trade.TradeStatus.Pending)
            {
                return BadRequest(new
                {
                    message = "Este intercambio ya ha sido aceptado o rechazado."
                });
            }

            // Si encontramos el intercambio y esta Pending, actualizamos el estado
            trade.Status = Trade.TradeStatus.Accepted;
            trade.LastUpdatedAt = DateTime.Now;

            // Guardamos los cambios
            await _context.SaveChangesAsync();

            var result = MapToTradeResponse(trade);

            return Ok(new
            {
                message = "Intercambio aceptado correctamente.",
                trade = result
            });

        }


		// ----------Reject----------

		// Método para rechazar un trade 
		[HttpPut("reject/{idTrade}")]
        public async Task<ActionResult> RejectTrade(int idTrade)
        {
            // Verificamos si el usuario esta logeado
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if(getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para rechazar un intercambio."
                });
            }

            int userId = int.Parse(getUserId.Value);

            // Buscamos el Trade
            var trade = await _context.Trades.FindAsync(idTrade);

            if (trade == null)
            {
                return NotFound(new
                {
                    message = "Intercambio no encontrado."
                });
            }

            // Comprobamos que el usuario logeado es el receptor del intercambio
            if(trade.ReceiverId != userId)
            {
                return Unauthorized(new
                {
                    message = "No puedes rechazar un intercambio que no es tuyo."
                });
            }

            // Solo se puede aceptar si el intercambio está pendiente
            if(trade.Status != Trade.TradeStatus.Pending)
            {
                return BadRequest(new
                {
                    message = "Este intercambio ya ha sido aceptado o rechazado."
                });
            }

            // Si encontramos el intercambio y esta Pending, actualizamos el estado
            trade.Status = Trade.TradeStatus.Rejected;
            trade.LastUpdatedAt = DateTime.Now;

            // Guardamos los cambios
            await _context.SaveChangesAsync();

			var result = MapToTradeResponse(trade);

			return Ok(new
            {
                message = "Intercambio rechazado correctamente.",
                trade = result
            });

        }


		// ----------Funciones----------

        // Funcion para mapear tradeResponse
        private TradeResponse MapToTradeResponse(Trade trade)
        {
            return new TradeResponse
            {
                Id = trade.Id,
                RequesterId = trade.RequesterId,
                RequesterUsername = trade.Requester?.Username ?? string.Empty,
                ReceiverId = trade.ReceiverId,
                ReceiverUsername = trade.Receiver?.Username ?? string.Empty,
                OfferedCard = trade.OfferedCard != null ? MapToCardResponse(trade.OfferedCard) : null!,
                RequestedCard = trade.RequestedCard != null ? MapToCardResponse(trade.RequestedCard) : null!,
                Status = trade.Status.ToString(),
                CreatedAt = trade.CreatedAt,
                LastUpdatedAt = trade.LastUpdatedAt,

			};
        }

		// FUncion para mapear CardResponse
		private CardResponse MapToCardResponse(Card card)
		{
			return new CardResponse
			{
				Id = card.Id,
				PokemonName = card.PokemonName,
				PokemonType = card.PokemonType,
				Rarity = card.Rarity,
				ImageUrl = card.ImageUrl,
				Collection = card.Collection,
				Graded = card.Graded,
				Grade = card.Grade,
				Price = card.Price,
				Is_tradeable = card.Is_tradeable,
				CreatedAt = card.CreatedAt,
				LastUpdatedAt = card.LastUpdatedAt,
				UserId = card.UserID,
				Username = card.User?.Username ?? string.Empty,
				Tags = card.CardTags.Select(ct => new TagResponse
				{
					Id = ct.Tag.Id,
					Name = ct.Tag.Name
				})
                .ToList()
			};
		}

        // Funcion para filtrado de los trades
        private IQueryable<Trade> ApplyTradeStatusFilter(IQueryable<Trade> query, string? status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return query;
            }

            status = status.ToLower();

            switch (status)
            {
                case "pending":
                    query = query.Where(t => t.Status == Trade.TradeStatus.Pending); 
                    break;

                case "accepted":
                    query = query.Where(t => t.Status == Trade.TradeStatus.Accepted); 
                    break;

                case "rejected":
                    query = query.Where(t => t.Status == Trade.TradeStatus.Rejected);
                    break;

                default:
                    break;
            }

            return query;

        }

        // Funcion para la ordenacion de los trades
        private IQueryable<Trade> ApplyTradeOrdering (IQueryable<Trade> query, string? orderBy)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                return query.OrderByDescending(t => t.CreatedAt);
            }

            orderBy = orderBy.ToLower();

            switch (orderBy)
            {
                case "fecha-asc":
                    return query.OrderBy(t => t.CreatedAt);

                case "fecha-desc":
                    return query.OrderByDescending(t => t.CreatedAt);

                case "status-asc": 
                    return query.OrderBy(t => t.Status.ToString());

                case "status-desc": 
                    return query.OrderByDescending(t => t.Status.ToString());

                // Si no seleccionamos nada lo ponemos por defecto
                default:
					return query.OrderByDescending(t => t.CreatedAt);
			}
        }

		// Funcion para el paginado
		private async Task<PagedResult<TradeResponse>> ApplyPagination(IQueryable<Trade> query, int page, int pageSize)
		{
			// Contamos cuantas cartas totales hay tras aplicar los filtros correspondientes y la ordenación
			var totalCount = await query.CountAsync();

			// Calculamos el número total de páginas 
			var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

			// Aplicamos paginación
			var trades = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			// Mapeamos la lista de trades con la funcion de CardResponse
			var items = trades.Select(MapToTradeResponse).ToList();

			// Y devolvemos el resultado 
			return new PagedResult<TradeResponse>
			{
				CurrentPage = page,
				PageSize = pageSize,
				TotalPages = totalPages,
				TotalCount = totalCount,
				Items = items

			};
		}
	}
}
