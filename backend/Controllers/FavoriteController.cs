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

    [Authorize] // Ponemos el authorize arriba para que todos los métodos lo implementen
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoriteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ----------Add----------

        // Método para añadir una carta a favoritos
        [HttpPost]
        public async Task<ActionResult<Favorite>> AddFavorite(AddFavoriteRequest request)
        {
			// Comprobamos que se realizan bien las validaciones de los DTOs
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Comprobamos si se ha iniciado sesión
			var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if (getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para realizar esta acción."
                });
            }

            // Guardamos el id del usuario
            var userId = int.Parse(getUserId.Value);

            // Verificamos si existe la carta 
            var cardExists = await _context.Cards.AnyAsync(c => c.Id == request.CardId);

            if (!cardExists)
            {
                return NotFound(new
                {
                    message = "Carta no encontrada."
                });  
            }

            // Verificamos si la carta ya está en favoritos
            // Aqui comprobamos si en la tabla favoritos esta tanto el id del usuario como el id de una carta en concreto
            var alreadyFavorite = await ExistsInFavorites(userId, request.CardId);

            if (alreadyFavorite)
            {
                return BadRequest(new
                {
                    message = "Esta carta ya está en tus favoritos."
                });
            }

            // Si la carta no esta en favoritos, la guardamos
            var favorite = new Favorite
            {
                UserId = userId,
                CardId = request.CardId,
                CreatedAt = DateTime.Now,
            };

            // La añadimos a la base de datos
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();


            return Ok(new
                {
                    message = "Carta añadida a favoritos ❤️"
                });
        }


        // ----------GetFavorites----------

        // Método para obtener todas las cartas de la lista de favoritos
        [HttpGet]
        public async Task<ActionResult> GetMyFavorites(
            int page = 1,
            string? pokemonType = null,
            string? collection = null,
            string? pokemonName = null,
            string? rarity = null,
            string? tagName = null,
            string? orderBy = null
            )
        {

            // Validamos los parámetros para que no fallen
            if (page < 1) page = 1;

            int pageSize = 10; // Siempre es 10

            // Verificamos si se ha iniciadio seson
            var userIdGet = User.FindFirst(ClaimTypes.NameIdentifier);

            if(userIdGet == null)
            {
                return NotFound(new
                {
                    message = "Debes iniciar sesión para realizar esta acción."
                });

            }

            // Guardamos el id del usuario
            int userId = int.Parse(userIdGet.Value);

            // Buscamos y listamos las cartas con el id del usuario en la base de datos y lo guardamos como IQuerable
            var query = _context.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => f.Card)
                .AsQueryable();

            // Aplicamos los filtros y el orden para ver las cartas
            query = ApplyFilters(query, pokemonType, collection, pokemonName, rarity, tagName);

            // Si no hay resultados
            if (!await query.AnyAsync())
            {
                return NotFound(new {
                    message = "No tienes cartas en tu lista de favoritos con esos filtros."
                });
            }

            // Aplicamos la paginación y el orden
            var result = ApplyPagination(query, page, pageSize, orderBy);

            // Devolvemos las cartas paginadas
            return Ok(result);
        }

        // Método para comprobar si una carta está en favoritos
        [HttpGet("is-favorite/{cardId}")]
        public async Task<ActionResult> IsCardFavorite(int cardId)
        {
            // Comprobamos que el usuario está logeado
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if (getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes inicar sesión para comprobar si la carta está en favoritos."
                });
            }

            int userId = int.Parse(getUserId.Value);

            // Comprobamos si la carta existe
            var cardExists = await _context.Cards.AnyAsync(c => c.Id == cardId);

            if (!cardExists)
            {
                return NotFound(new
                {
                    message = "Carta no encontrada."
                });
            }

            // Comprobamos si está en favoritos
            bool isFavorite = await ExistsInFavorites(userId, cardId);

            return Ok(new
            {
                cardId = cardId,
                isFavorite = isFavorite
            });
        }

        // ----------Delete----------

        // Método para eliminar una carta de faovritos
        [HttpDelete("{cardId}")]
        public async Task<ActionResult<Card>> DeleteFavorite(int cardId)
        {
            // Comprobamos que el usuario está logeado
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if (getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para realizar esta acción."
                });
            }

            // Guardamos el id del usuario
            int userId = int.Parse(getUserId.Value);

            // Buscamos el favorito que coincida con el userId y cardId
            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.CardId == cardId);

            // Si no encontramos la carta
            if(favorite == null)
            {
                return NotFound(new
                {
                    message = "La carta no está en tus favoritos."
                });
            }

            // Si la encontramos la eliminamos
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Carta eliminada de favoritos correctamente."
            });
        }


        // ----------Funciones----------

        // Creamos la funcion para el CardResponse mapeado para luego ponerlo como respuesta
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
                }).ToList()
            };
        }

		// Funcion para el paginado de las cartas
		private async Task<PagedResult<CardResponse>> ApplyPagination(
			 IQueryable<Card> query,
			 int page,
			 int pageSize,
			 string? orderBy = null
			 )
		{
			// Contamos cuantas cartas totales hay tras aplicar los filtros correspondientes y la ordenación
			var totalCount = await query.CountAsync();

			// Calculamos el número total de páginas 
			var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

			// Obtenemos la lista de cartas para ordenarlas 
			var cards = await query
				.Include(c => c.User)
				.Include(c => c.CardTags)
					.ThenInclude(ct => ct.Tag)
				.ToListAsync();

			// LLamamos a la función para ordenar las cartas
			cards = ApplyOrdering(cards, orderBy);

			// Después de ordenar paginamos las cartas
			cards = cards.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			// Mapeamos la lista de cartas con la funcion de CardResponse
			var items = cards.Select(MapToCardResponse).ToList();

			// Y devolvemos el resultado 
			return new PagedResult<CardResponse>
			{
				CurrentPage = page,
				PageSize = pageSize,
				TotalPages = totalPages,
				TotalCount = totalCount,
				Items = items

			};
		}

		// Funcion para aplicar los filtros a las cartas
		private IQueryable<Card> ApplyFilters(
            IQueryable<Card> query, 
            string? pokemonType, 
            string? collection, 
            string? pokemonName, 
            string? rarity,
            string? tagName
            )
        {
            // Primer filtro, filtrar por el tipo del pokemon
            if(!string.IsNullOrEmpty(pokemonType))
            {
                query = query.Where(c => c.PokemonType.ToLower().Contains(pokemonType.ToLower()));
            }

            // Filtrar por la coleccion de la carta
            if(!string.IsNullOrEmpty(collection))
            {
                query = query.Where(c => c.Collection.ToLower().Contains(collection.ToLower()));
            }

            // Filtrar por el nombre del Pokémon
            if(!string.IsNullOrEmpty(pokemonName))
            {
                query = query.Where(c => c.PokemonName.ToLower().Contains(pokemonName.ToLower()));
            }

            // Filtro por la rareza de la carta 
            if(!string.IsNullOrEmpty(rarity))
            {
                query = query.Where(c => c.Rarity.ToLower().Contains(rarity.ToLower()));
            }

            // Filtro por nombre del tag
            if (!string.IsNullOrEmpty(tagName))
            {
                query = query.Where(c => c.CardTags.Any(ct => ct.Tag.Name.ToLower().Contains(tagName.ToLower())));
            }

            return query;
        }

        // Función para ordenar las cartas
		private List<Card> ApplyOrdering(List<Card> cards, string? orderBy)
		{
			// Si el parametro no esta vacío
			if (!string.IsNullOrEmpty(orderBy))
			{
				orderBy = orderBy.ToLower();

				// Ordenamos los datos segun el tipo de orden que nos venga por parametro
				switch (orderBy)
				{
					case "fecha-asc":
						return cards.OrderBy(c => c.CreatedAt).ToList();

					case "fecha-desc":
						return cards.OrderByDescending(c => c.CreatedAt).ToList();

					case "nombre":
						return cards.OrderBy(c => c.PokemonName).ToList();

					case "rareza":
						return cards.OrderBy(c => GetRarityOrder(c.Rarity)).ToList();

					case "tipo":
						return cards.OrderBy(c => GetPokemonTypeOrder(c.PokemonType)).ToList();

					default:
						return cards.OrderByDescending(c => c.CreatedAt).ToList();
				}

			}
			else
			{
				// Si no se pone nada lo ponemos por fecha descendente por defecto
				return cards.OrderByDescending(c => c.CreatedAt).ToList();

			}

		}

		// Función para el orden personalizado por tipo
		private int GetPokemonTypeOrder(string type)
		{
			var orderByType = new List<string>
			{
				"planta", "fuego", "agua", "eléctrico", "psíquico", "lucha",
				"siniestro", "acero", "dragón", "normal", "entrenador"
			};

			var lowerType = type.ToLower();

			return orderByType.IndexOf(lowerType) >= 0 ? orderByType.IndexOf(lowerType) : orderByType.Count;
		}

		// Función para el orden personalizado por rareza
		private int GetRarityOrder(string rarity)
		{
			var orderByRarity = new List<string>
			{
				"comun", "infrecuente", "rara", "rara ultra", "rara ilustracion", "rara ilustracion especial", "rara hiper"
			};

			var lowerRarity = rarity.ToLower();

			return orderByRarity.IndexOf(lowerRarity) >= 0 ? orderByRarity.IndexOf(lowerRarity) : orderByRarity.Count;
		}

		// Funcion para comprobar si la carta esta en favoritos
		private async Task<bool> ExistsInFavorites(int userId, int cardId)
        {
            return await _context.Favorites.AnyAsync(f => f.UserId == userId && f.CardId == cardId);
        }
    }
}
