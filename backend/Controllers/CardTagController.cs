using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokeMarket.Data;
using PokeMarket.Models;
using PokeMarket.Models.DTOs;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace PokeMarket.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CardTagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CardTagController (ApplicationDbContext context)
        {
            _context = context;
        }

        // ----------Add----------

        // Método para añadir un tag a una carta del usuario, este tiene que estar logeado
        [HttpPost("add")]
        public async Task<ActionResult> AddTagToCard(AddTagToCardRequest request)
        {

            // Comprobamos si el usuario está logueado
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if (getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para realizar esta acción."
                });
            }

            int userId = int.Parse(getUserId.Value);

            // Verificamos si la carta existe y es del usuario logeado
            var card = await _context.Cards
                .FirstOrDefaultAsync(c => c.Id == request.CardId && c.UserID == userId);

            if (card == null)
            {   
                return NotFound(new
                {
                    message = "Carta no encontrada o no te pertenece."
                });
            }

            // Verificamos si el tag existe 
            var tagExists = await _context.Tags.AnyAsync(t => t.Id == request.TagId);

            if (!tagExists)
            {
                return NotFound(new
                {
                    message = "Tag no encontrado."
                });
            }

            // Verificamos si la carta ya tiene ese tag
            var cardTagExists = await _context.CardTags
                .AnyAsync(ct => ct.CardId == request.CardId && ct.TagId == request.TagId);

            if (cardTagExists)
            {
                return BadRequest(new
                {
                    message = "Este tag ya está asignado a esta carta."
                });
            }

            // Si no esta asociado a ninguna carta
            // Creamos un nuevo vínculo
            var newCardTag = new CardTag
            {
                CardId = request.CardId,
                TagId = request.TagId,
            };

            // Añadimos a la base de datos
            _context.CardTags.Add(newCardTag);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tag añadido correctamente a la carta."
            });
        }

        // ----------GetCardTags----------

        // Método para obtener todas las relaciones CardTag
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult> GetAllCardTags(
            int page = 1,
            string? pokemonType = null,
            string? collection = null,
            string? pokemonName = null,
            string? rarity = null,
            string? tagName = null,
            string? orderBy = null
            )
        {
            if (page < 1) page = 1;

            int pageSize = 10;

            // Obtenemos de la consulta de base de datos las cartas relacionadas a tags
            var query = _context.CardTags
                 .Include(ct => ct.Card)
                     .ThenInclude(c => c.User)
                 .Include(ct => ct.Card)
                     .ThenInclude(c => c.CardTags)
                         .ThenInclude(ct => ct.Tag)
                 .Select(ct => ct.Card)
                 .Distinct()
                 .AsQueryable();

            // Aplicamos los filtros y el orden
            query = ApplyCardFilters(query, pokemonType, collection, pokemonName, rarity, tagName);

            // Si no hay resultados 
            if (!await query.AnyAsync())
            {
                return NotFound(new
                {
                    message = "No se han encontrado cartas con tags asignados y los filtros seleccionados."
                });
            }


			// Aqui aplicamos el paginado y ordenamos las cartas
			var result = await ApplyCardPagination(query, page, pageSize, orderBy);

			return Ok(result);
        }


        // ----------GetCards----------

        // Método para ver todas las cartas con un tag en especifico
        [HttpGet("tag/{tagId}")]
        public async Task<ActionResult> GetCardsWithTag(
            int tagId,
            int page = 1,
            string? pokemonType = null,
            string? collection = null,
            string? pokemonName = null,
            string? rarity = null,
            string? tagName = null,
            string? orderBy = null
            )
        {
            if (page < 1) page = 1;

            int pageSize = 10;

            // Verificamos si el tag existe 
            var tagExists = await _context.Tags.AnyAsync(t => t.Id == tagId);

            if (!tagExists)
            {
                return NotFound(new
                {
                    message = "Tag no encontrado."
                });
            }

            // Creamos la consulta para obtener todas las cartas que tengan el tag seleccionado
            var query = _context.CardTags
                .Where(ct => ct.TagId == tagId)
                .Select(ct => ct.Card)
                .Include(c => c.User)
                .Include(c => c.CardTags)
                    .ThenInclude(ct => ct.Tag)
                .AsQueryable();

            // Aplicamos los filtros
            query = ApplyCardFilters(query, pokemonType, collection, pokemonName, rarity, tagName);

            // Si no hay resultadows
            if (!await query.AnyAsync())
            {
                return NotFound(new
                {
                    message = "No se han encontrado cartas con esos filtros."
                });
            }


			// Aqui aplicamos el paginado y ordenamos las cartas
			var result = await ApplyCardPagination(query, page, pageSize, orderBy);

			return Ok(result);
        }

        // Método para obtener las cartas con algun tag del usuario logeado
        [Authorize]
        [HttpGet("my-cards-with-tags")]
        public async Task<ActionResult> GetMyCardsWithTags(
            int page = 1,
            string? pokemonType = null,
            string? collection = null,
            string? pokemonName = null,
            string? rarity = null,
            string? tagName = null,
            string? orderBy = null
            )
        {
            if (page < 1) page = 1;
            int pageSize = 10;

            // Obtenemos el id del user
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if (getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para poder ver tus tags."
                });
            }

            int userId = int.Parse(getUserId.Value);

            // Obtenemos solo las cartas del usuario que tengan al menos un tag asignado
            var query = _context.Cards
                .Where(c => c.UserID == userId && c.CardTags.Any())
                .Include(c => c.User)
                .Include(c => c.CardTags)
                    .ThenInclude(ct => ct.Tag)
                .AsQueryable();

            // Aplicamos filtros y orden
            query = ApplyCardFilters(query, pokemonType, collection, pokemonName, rarity, tagName);

            // Si no hay resultados 
            if (!await query.AnyAsync())
            {
                return NotFound(new
                {
                    message = "No se han encontrado cartas con tags asignados y los filtros seleccionados."
                });
            }

			// Aqui aplicamos el paginado y ordenamos las cartas
			var result = await ApplyCardPagination(query, page, pageSize, orderBy);

			return Ok(result);

        }


        // ----------GetTags----------

        // Método para ver los tags de una carta con su Id
        [HttpGet("card/{cardId}")]
        // No ponemos dentro de Task <CardTag> porque no devolvemos un objeto
        public async Task<ActionResult> GetTagsFromCard(
            int cardId,
            int page = 1,
            string? searchTag = null,
            string? orderBy = null
            )
        {
            if (page < 1) page = 1;

            int pageSize = 10;

            // Verificamos si existe la carta
            var cardExists = await _context.Cards.AnyAsync(c => c.Id == cardId);

            if (!cardExists)
            {
                return NotFound(new
                {
                    message = "Carta no encontrada."
                });
            }

            // Buscamos los tags que tengan relacion con la carta
            var tagQuery = _context.CardTags
                .Where(ct => ct.CardId == cardId)
                .Include(ct => ct.Tag)
                .Select(ct => ct.Tag)
                .AsQueryable();

            // Aplicamos filtros y orden
            tagQuery = ApplyTagFiltersAndOrdering(tagQuery, searchTag, orderBy);

            var result = await ApplyTagPagination(tagQuery, page, pageSize);

            return Ok(result);
        }

        // Método para devolver todos los tags que un usuario ha utilizado en sus cartas
        [Authorize]
        [HttpGet("my-tags")]
        public async Task<ActionResult> GetTagsByUserCards(
            int page = 1,
            string? searchTag = null,
            string? orderBy = null
            )
        {
            if (page < 1) page = 1;
            int pageSize = 10;

            // Obtenemos el id del user
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if (getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para poder ver tus tags."
                });
            }

            int userId = int.Parse(getUserId.Value);

            // Obtenemos los tags de todas las cartas del usuario
            var tagQuery = _context.Cards
                .Where(c => c.UserID == userId)
                .SelectMany(c => c.CardTags)
                .Select(ct => ct.Tag)
                .Distinct()
                .AsQueryable();

            // Aplicamos los filtros y la ordenacion
            tagQuery = ApplyTagFiltersAndOrdering(tagQuery, searchTag, orderBy);

            // Aplicamos la paginación
            var result = await ApplyTagPagination(tagQuery, page, pageSize);

            return Ok(result);

        }


        // ----------Delete----------

        // Método para eliminar un tag de una carta
        [Authorize]
        [HttpDelete("{cardId}/{tagId}")]
        public async Task<ActionResult> DeleteTagFromCard(int cardId, int tagId)
        {
            // Comprobamos si el usuario está logeado
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if (getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para realizar esta acción."
                });
            }

            int userId = int.Parse(getUserId.Value);

            // Verificamos que la carta pertenezca al usuario logeado
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == cardId && c.UserID == userId);

            if (card == null)
            {
                return NotFound(new
                {
                    message = "Carta no encontrada o no te pertenece."
                });
            }

            // Buscamos si la carta tiene el tag que queremos eliminarle
            var cardTag = await _context.CardTags.FirstOrDefaultAsync(ct => ct.CardId == cardId && ct.TagId == tagId);

            if (cardTag == null)
            {
                return NotFound(new
                {
                    message = "El tag no está asignado a esta carta."
                });
            }

            // Eliminamos el tag de la carta
            _context.CardTags.Remove(cardTag);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tag eliminado correctamente de la carta."
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
        private async Task<PagedResult<CardResponse>> ApplyCardPagination(
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
        private IQueryable<Card> ApplyCardFilters(
            IQueryable<Card> query,
            string? pokemonType,
            string? collection,
            string? pokemonName,
            string? rarity,
            string? tagName
            )
        {
            // Primer filtro, filtrar por el tipo del pokemon
            if (!string.IsNullOrEmpty(pokemonType))
            {
                query = query.Where(c => c.PokemonType.ToLower().Contains(pokemonType.ToLower()));
            }

            // Filtrar por la coleccion de la carta
            if (!string.IsNullOrEmpty(collection))
            {
                query = query.Where(c => c.Collection.ToLower().Contains(collection.ToLower()));
            }

            // Filtrar por el nombre del Pokémon
            if (!string.IsNullOrEmpty(pokemonName))
            {
                query = query.Where(c => c.PokemonName.ToLower().Contains(pokemonName.ToLower()));
            }

            // Filtro por la rareza de la carta 
            if (!string.IsNullOrEmpty(rarity))
            {
                query = query.Where(c => c.Rarity.ToLower().Contains(rarity.ToLower()));
            }

            // Filtro por nombre del tag
            if (!string.IsNullOrEmpty(tagName))
            {
                query = query.Where(c => c.CardTags.Any(ct => ct.Tag.Name.ToLower().Contains(tagName .ToLower())));
            }

            return query;
        }

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

		// Función para ordenar y filtrar por nombre los tags
		private IQueryable<Tag> ApplyTagFiltersAndOrdering(
            IQueryable<Tag> query,
            string? searchTag,
            string? orderBy
            )
        {
            // Filtro por el nombre del tag
            if (!string.IsNullOrEmpty(searchTag))
            {
                query = query.Where(t => t.Name.ToLower().Contains(searchTag.ToLower()));
            }

            // Ordenamos por el nombre
            orderBy = orderBy?.ToLower();

            switch (orderBy)
            {
                case "nombre-desc":
                    query = query.OrderByDescending(t => t.Name);
                    break;

                case "nombre-asc":
                    query = query.OrderBy(t => t.Name);
                    break;

                default:
                    query = query.OrderBy(t => t.Name);
                    break;

            }

            return query;
        }

        private async Task<PagedResult<TagResponse>> ApplyTagPagination(
            IQueryable<Tag> query,
            int page,
            int pageSize
            )
        {
            int totalCount = await query.CountAsync();

            int totalPages = (int)Math.Ceiling((double) totalCount / pageSize);

            var tags = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TagResponse
                {
                    Id = t.Id,
                    Name = t.Name,
                })
                .ToListAsync();

            return new PagedResult<TagResponse>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = tags
            };
        }

    }
}
