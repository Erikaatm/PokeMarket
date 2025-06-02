using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokeMarket.Data;
using PokeMarket.Models;
using PokeMarket.Models.DTOs;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PokeMarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ----------Create-----------

        // Metodo para crear una carta
        // Solo los usuarios logeados pueden crear una carta
        [Authorize]
        [HttpPost] // Se hace una petición POST para crear cartas nuevas
        public async Task<ActionResult> CreateCard(CreateCardRequest request)
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

            // Validamos que los tags que hemos puesto existen
            var tagsExists = await _context.Tags
                .Where(t => request.TagIds.Contains(t.Id))
                .ToListAsync();

            if(tagsExists.Count != request.TagIds.Count)
            {
                return BadRequest(new
                {
                    message = "Uno o más tags no existen."
                });
            }

            // Creamos una nueva instancia Card con los datos que tenemos del DTO
            var card = new Card
            {
                UserID = userId,
                PokemonName = request.PokemonName,
                PokemonType = request.PokemonType,
                Rarity = request.Rarity,
                ImageUrl = request.ImageUrl,
                Collection = request.Collection,
                Graded = request.Graded,
                Grade = request.Graded ? request.Grade : 0,
                Price = request.Price,
                Is_tradeable = request.Is_tradeable,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now,

            };

            // Guardamos en la base de datos Cards
            _context.Cards.Add(card);

            // Guardamos los cambios
            await _context.SaveChangesAsync();

            // Relacionamos los tags con la carta
            foreach (var tag in tagsExists)
            {
                var cardTag = new CardTag
                {
                    CardId = card.Id,
                    TagId = tag.Id
                };

                // Añadimos a la base de datos de CardTag
                _context.CardTags.Add(cardTag);

            }
            
            // Guardamos los cambios
            await _context.SaveChangesAsync();

            // Volvemos a cargar la carta para que salga con todos los datos de esta
            var createdCard = await _context.Cards
                .Include(c => c.User)
                .Include(c => c.CardTags)
                    .ThenInclude(ct => ct.Tag)
                .FirstOrDefaultAsync(c => c.Id == card.Id);

            if (createdCard == null)
            {
                return NotFound(new
                {
                    message = "Carta no encontrada."
                });
            }

            // Lo mapeamos
            var response = MapToCardResponse(createdCard);

            return CreatedAtAction(nameof (GetCardByID), new { id = card.Id }, new
            {
                message = "Carta creada correctamente.",
                card = response
            });
            
        }


        // ----------GetCards-----------

        // Método que obtiene todas las cartas 
        [HttpGet] // Responde a una petición GET 
        public async Task<ActionResult> GetCards(
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

            // Obtenemos todas las cartas de la base de datos
            var query = _context.Cards
                .Include(c => c.User)
                .Include(c => c.CardTags)
                    .ThenInclude(ct => ct.Tag)
                .AsQueryable(); // Esto nos permite construir una consulta que podemos modificar después para el paginado

            // Aplicamos los filtros que hemos puesto, si no ponemos ningun filtro se muestran todas las cartas por defecto
            query = ApplyFilters(query, pokemonType, collection, pokemonName, rarity, tagName);

            // Si no hay resultados
            if (!await query.AnyAsync()) {
                return NotFound(new
                {
                    message = "No se han encontrado cartas con esos filtros."
                });
            }

			// Aqui aplicamos el paginado y ordenamos las cartas
			var result = await ApplyPagination(query, page, pageSize, orderBy);

			return Ok(result);

        }

        // Método para obtener cartas a partir de su ID
        [HttpGet("{id}")] // Hacemos la petición GET a partir de un ID
        public async Task<ActionResult> GetCardByID(int id)
        {
            // Guardamos la carta que hemos encontrado en una variable
            var selectedCard = await _context.Cards
                .Include(c => c.User)
                .Include(c => c.CardTags)
                    .ThenInclude(ct => ct.Tag)
                .FirstOrDefaultAsync(c => c.Id == id); // Buscamos la carta dentro de la base de datos por el id

            if (selectedCard == null)
            {
                return NotFound(new
                {
                    message = "Carta no encontrada."
                });
            }

            // Aqui no se utiliza el .Select pq no devolvemos varias cartas
            var response = MapToCardResponse(selectedCard);

            return Ok(new
            {
                card = response,
            });
        }

        // Método para obtener todas las cartas de un usuario logeado
        [Authorize]
        [HttpGet("my-cards")]
        public async Task<ActionResult> GetMyCards(
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

            // Obtenemos el ID de la persona que se ha logeado desde el token
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            // Comprobamos que no sea null
            if (getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para realizar esta acción."
                });
            }

            // Guardamos el ID del usuario logeado en una variable
            var userID = int.Parse(getUserId.Value);

            // Creamos la consulta a la base de datos solo con las cartas del usuario logeado
            var query = _context.Cards
                .Where(c => c.UserID == userID)
                .Include(c => c.User)
                .Include(c => c.CardTags)
                    .ThenInclude(ct => ct.Tag)
                .AsQueryable();

            // Aplicamos los filtros que hemos puesto, si no ponemos ningun filtro se muestran todas las cartas por defecto
            query = ApplyFilters(query, pokemonType, collection, pokemonName, rarity, tagName);

            // Si no hay resultados
            if (!await query.AnyAsync())
            {
                return NotFound(new
                {
                    message = "No se han encontrado cartas con esos filtros."
                });
            }

			// Aqui aplicamos el paginado y ordenamos las cartas
			var result = await ApplyPagination(query, page, pageSize, orderBy);

            return Ok(result);

        }

        // Método para obtener las cartas del usuario logeado por el ID
        [Authorize]
        [HttpGet("my-cards/{id}")]
        public async Task<ActionResult> GetMyCardByID(int id)
        {
            // Verificamos que el usuario se haya logeado
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier); // Buscamos en la base de datos con el token

            if(getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para realizar esta acción."
                });
            }

            // Si lo encontramos guardamos su id en una vairable
            int userID = int.Parse(getUserId.Value);

            // Guardamos la carta que buscamos por el ID de la carta y el ID del usuario logeado en una variable
            var card = await _context.Cards
                .Include(c => c.User)
                .Include(c => c.CardTags)
                    .ThenInclude(ct => ct.Tag)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserID == userID);


            if (card == null)
            {
                return NotFound(new
                {
                    message = "Carta no encontrada."
                });
            }

            var response = MapToCardResponse(card);

            return Ok(new
            {
                card = response
            });
        }



        // Obtenemos las cartas de un usuario en específico por su ID
        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetCardsByUserId(
            int userId, 
            int page = 1,
            string? pokemonType = null,
            string? collection = null,
            string? pokemonName = null,
            string? rarity = null,
            string? tagName = null,
            string? orderBy = null)
        {
            // Validamos los parámetros para que no fallen
            if (page < 1) page = 1;

            int pageSize = 10;

            // Comprobamos si el usuario existe
            bool userExists = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!userExists)
            {
                return NotFound(new
                {
                    message = "El usuario que ha buscado no existe."
                });
            }

            // Creamos la consulta a la base de datos para obtener las cartas de un usuario en especifico
            var query = _context.Cards
                .Where(c => c.UserID == userId)
                .Include(c => c.User)
                .Include(c => c.CardTags)
                    .ThenInclude(ct => ct.Tag)
                .AsQueryable();

            // Aplicamos los filtros que hemos puesto, si no ponemos ningun filtro se muestran todas las cartas por defecto
            query = ApplyFilters(query, pokemonType, collection, pokemonName, rarity, tagName);

            // Si no hay resultados
            if (!await query.AnyAsync())
            {
                return NotFound(new
                {
                    message = "No se han encontrado cartas con esos filtros."
                });
            }

            // Aqui aplicamos el paginado y ordenamos las cartas
            var result = await ApplyPagination(query, page, pageSize, orderBy);

            return Ok(result);
        }


        // ----------Update-----------

        // Método para actualizar una carta
        [Authorize]
        [HttpPut("{id}")] // Hacemos una petición PUT para actualizar una carta con su ID
        public async Task<ActionResult> UpdateCard(int id, UpdateCardRequest updateCard)
        {
            // Comprobamos que se realizan bien las validaciones de los DTOs
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Buscamos la carta existente
            var card = await _context.Cards
                .Include (c => c.User)
                .Include(c => c.CardTags)
                    .ThenInclude(ct => ct.Tag)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (card == null)
            {
                return NotFound(new { message = "Carta no encontrada." });
            }

            // Vemos si el usuario está logeado para actualizar la carta
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier); // Buscamos
            if(getUserId == null || card.UserID != int.Parse(getUserId.Value))
            {
                return Unauthorized(new
                {
                    message = "No tienes permisos para modificar la carta."
                });
            }

            // Actualizamos los datos si están rellenos unicamente
            card.PokemonName = !string.IsNullOrEmpty(updateCard.PokemonName) ? updateCard.PokemonName : card.PokemonName;
            card.PokemonType = !string.IsNullOrEmpty(updateCard.PokemonType) ? updateCard.PokemonType : card.PokemonType;
            card.Rarity = !string.IsNullOrEmpty(updateCard.Rarity) ? updateCard.Rarity : card.Rarity;
            card.ImageUrl = !string.IsNullOrEmpty(updateCard.ImageUrl) ? updateCard.ImageUrl : card.ImageUrl;
            card.Collection = !string.IsNullOrEmpty(updateCard.Collection) ? updateCard.Collection : card.Collection;

            // Para los campos que son booleanos y enteros, comprobamos con valores distintos por defecto 
            card.Graded = updateCard.Graded;
            card.Grade = updateCard.Graded ? updateCard.Grade : 0; // Si no esta gradeada lo dejamos en 0
            card.Price = updateCard.Price > 0 ? updateCard.Price : card.Price;
            card.Is_tradeable = updateCard.Is_tradeable;
            card.LastUpdatedAt = DateTime.Now;

            // Validamos los tags nuevos
            var tagsExists = await _context.Tags
                .Where(t => updateCard.TagIds.Contains(t.Id))
                .ToListAsync();

            if(tagsExists.Count != updateCard.TagIds.Count)
            {
                return BadRequest(new
                {
                    message = "Uno o más tags no existen."
                });
            }

            // Comparamos los tags actuales con los nuevos tags
            var currentTagsIds = card.CardTags.Select(ct => ct.TagId).ToList();
            var newTags = updateCard.TagIds.Distinct().ToList();

            // Comparamos los dos
            bool tagsChanged = false;

            if(currentTagsIds.Count != newTags.Count)
            {
                tagsChanged = true;
            } else
            {
                foreach(var idT in updateCard.TagIds)
                {
                    if (!currentTagsIds.Contains(idT))
                    {
                        tagsChanged = true;
                        break;
                    }
                }
            }

            // Si los tags han cambiado, los reemplazamos
            if(tagsChanged)
            {
                // Primero eliminamos los tags
                var deleteTags = _context.CardTags
                    .Where(ct => ct.CardId == card.Id);

                _context.CardTags.RemoveRange(deleteTags);

                // Y añadimos los nuevos
                foreach(var tag in tagsExists)
                {
                    var cardTag = new CardTag
                    {
                        CardId = card.Id,
                        TagId = tag.Id,
                    };

                    _context.CardTags.Add(cardTag);
                }
            }

            await _context.SaveChangesAsync(); // Guardamos los cambios

            // Recargamos la carta con todo lo que hemos incluido
            var updatedCard = await _context.Cards
                .Include(c => c.User)
                .Include(c => c.CardTags)
                    .ThenInclude(ct => ct.Tag)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (updatedCard == null)
            {
                return NotFound(new
                {
                    message = "Carta no encontrada."
                });
            }

            var response = MapToCardResponse(updatedCard);

            return Ok(new
            {
                message = "Carta modificada correctamente",
                card = response
            });

        }

        // ----------Delete-----------

        // Método para eliminar una carta
        [Authorize]
        [HttpDelete("{id}")] // Hacemos una petición de DELETE pasando el id de la carta que queramos
        public async Task<ActionResult> DeleteCard(int id)
        {
            // Buscamos la carta en la tabla con el id
            var card = await _context.Cards.FindAsync(id);

            // Si la carta es nula devolvemos un mensaje de error
            if(card == null)
            {
                return NotFound(new
                {
                    message = "Carta no encontrada."
                });
            }

            // Verificamos que la carta es del usuario logeado
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);
            if(getUserId == null || card.UserID != int.Parse(getUserId.Value))
            {
                return Unauthorized(new
                {
                    message = "No tienes permisos para eliminar esta carta."
                });
            }

            // Si la encontramos, la eliminamos 
            _context.Cards.Remove(card); // Eliminamos la carta que hemos obtenido con el ID
            // Guardamos los cambios
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Carta eliminada. "
            });
        }


        // ----------Funciones-----------

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

    }
}
