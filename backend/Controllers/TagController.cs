using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokeMarket.Data;
using PokeMarket.Models;
using PokeMarket.Models.DTOs;

namespace PokeMarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TagController(ApplicationDbContext context)
        {
            _context = context;
        }

		// ----------Create----------

		// Método para crear Tags, solo para admins
		[Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<Tag>> CreateTag(Tag tag)
        {
            // Validamos que el nombre del Tag no este vacío
            if (string.IsNullOrEmpty(tag.Name))
            {
                return BadRequest(new
                {
                    message = "El nombre del tag es obligatorio."
                });
            }

            // Verificar si ya existe un Tag con ese nombre
            bool tagExists = await _context.Tags.AnyAsync(t => t.Name.ToLower() == tag.Name.ToLower());

            if (tagExists)
            {
                return BadRequest(new
                {
                    message = "Ya existe un Tag con ese nombre."
                });
            }

            // Si no existe
            tag.CreatedAt = DateTime.Now;

            // Añadimos a la base de datos
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tag creado correctamente.",
                tag
            });

        }


		// ----------GetAllTags----------

		// Método para obtener todos los tags
		[HttpGet]
        public async Task<ActionResult> GetAllTags(
			int page = 1,
			string? searchTag = null,
			string? orderBy = null
			)
        {
			int pageSize = 10;

			if (page < 1) page = 1;

            // Hacemos la consulta para obtener todos los tags de la base de datos
            var query = _context.Tags
				.AsQueryable();

			// Aplicamos los filtros de los tags
			query = ApplyTagFiltersAndOrdering(query, searchTag, orderBy);

			// Validamos si no esta vacío
			if (!await query.AnyAsync())
			{
				return NotFound(new
				{
					message = "No se han encontrado tags con esos filtros."
				});
			}

			// Paginamos los tags
			var result = await ApplyTagPagination(query, page, pageSize);

            return Ok(result);
        }


		// ----------GetTagId----------

		// Método para obtener un tag por id
		[HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTagById(
			int id,
			int page = 1,
			string? pokemonType = null,
			string? collection = null,
			string? pokemonName = null,
			string? rarity = null,
			string? orderBy = null

			)
        {
			int pageSize = 10;

			if (page < 1) page = 1;

			// Obtenemos de la base de datos el tag y todas las cartas relacionadas con ese tag
			var tag = await _context.Tags
				.Include(t => t.CardTags)
					.ThenInclude(ct => ct.Card)
						.ThenInclude(c => c.User)
				.Include(t => t.CardTags)
					.ThenInclude(ct => ct.Card)
						.ThenInclude(c => c.CardTags)
							.ThenInclude(ct => ct.Tag)
                .FirstOrDefaultAsync(t => t.Id == id);

            if(tag == null)
            {
                return NotFound(new
                {
                    message = "Tag no encontrado."
                });
            }

			// Sacamos las cartas que estan relacionadas con ese tag
			var cardsQuery = tag.CardTags
				.Select(ct => ct.Card)
				.AsQueryable();

			// Aplicamos los filtros, la ordenación y la paginación a las cartas
			cardsQuery = ApplyCardFilters(cardsQuery, pokemonType, collection, pokemonName, rarity);
			var result = await ApplyCardPagination(cardsQuery, page, pageSize, orderBy);

            return Ok(result);
        }


		// ----------Update----------

		// Método para actualizar un tag
		[Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Tag>> UpdateTag(int id, UpdateTagRequest updatedTag) 
        {
            // Buscamos el tag dentro de la base de datos
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound(new
                {
                    message = "Tag no encontrado."
                });
            }

            // Actualizamos solo si el campo no esta vacío
            tag.Name = !string.IsNullOrEmpty(updatedTag.Name) ? updatedTag.Name : tag.Name;

            tag.LastUpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tag actualizado correctamente.",
                tag = tag
            });
        }


		// ----------Delete----------

		// Método para eliminar un tag
		[Authorize(Roles ="admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTag(int id)
        {
            // Buscamos el tag en la base de datos
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound(new
                {
                    message = "Tag no encontrado."
                });
            }

            // Eliminamos el tag
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tag eliminado correctamente.",
                tag = tag
            });
        }


		// ----------Funciones----------
        
        // Funcion para paginar los tags
		private async Task<PagedResult<TagResponse>> ApplyTagPagination(
		   IQueryable<Tag> query,
		   int page,
		   int pageSize
		   )
		{
			int totalCount = await query.CountAsync();

			int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

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

		// Funcion para filtrar y ordenar los tags 
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
			cards = ApplyCardOrdering(cards, orderBy);

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
			string? rarity
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

			return query;
		}

		// Función para ordenar las cartas
		private List<Card> ApplyCardOrdering(List<Card> cards, string? orderBy)
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


	}

}
