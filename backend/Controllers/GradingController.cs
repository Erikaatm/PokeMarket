using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class GradingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GradingController(ApplicationDbContext context)
        {
            _context = context;
        }

		// ----------Gradear----------

		// Método para gradear un gradeo
		[Authorize(Roles = "admin, grader")] // Estos métodos son solo para los admins
		[HttpPost]
        public async Task<ActionResult> GradeCard(GradeCardRequest request)
        {
			// Comprobamos que se realizan bien las validaciones de los DTOs
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Comprobamos que la carta existe
			var card = await _context.Cards.FindAsync(request.CardId);

            if (card == null)
            {
                return NotFound(new
                {
                    message = "Carta no encontrada."
                });
            }

            // Comprobamos si la carta ha sido gradeada
            if (card.Graded)
            {
                return BadRequest(new
                {
                    message = "Esta carta ya ha sido gradeada."
                });
            }

            // Validamos la nota de gradeo
            if (request.Grade < 1 || request.Grade > 10)
            {
                return BadRequest(new
                {
                    message = "El gradeo debe estar entre 1 y 10."
                });
            }

            // Obtenemos el ID del admin que ha hecho el gradeo
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if (getUserId == null)
            {
                return Unauthorized(new 
                {
                    message = "Debes iniciar sesión para realizar esta acción."
                });
            }

            // Guardamos el id en una variable
            int adminId = int.Parse(getUserId.Value);

            // Buscamos al admin entero con el id pq si no, nos da error de conversion
            var adminUser = await _context.Users.FindAsync(adminId);

			if (adminUser == null)
			{
				return NotFound(new { message = "Usuario que realiza el gradeo no encontrado." });
			}

			// Creamos un nuevo gradeo
			var grading = new Grading
            {
                CardId = request.CardId,
                Grade = request.Grade,
                GradedBy = adminUser,
                GradedAt = DateTime.Now,
            };

            // Añadimos el gradeo a la base de datos
            _context.Gradings.Add(grading);

            // Actualizamos la carta original
            card.Graded = true;
            card.Grade = request.Grade;
			card.GradeStatus = "accepted";
			card.LastUpdatedAt = DateTime.Now;

            // Guardamos los cambios
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Carta gradeada correctamente.",
                grading = grading
            });

        }

        // ----------RequestGrading----------

        // Método para que los usuarios puedan pedir un gradeo
        [Authorize] // Para usuarios
        [HttpPost("request")]
        public async Task<ActionResult> RequestGrading(int cardId)
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

            // Guardamos su Id
			int userId = int.Parse(getUserId.Value);

            // Buscamos si la carta solicitada existe en el perfil del usuario
            var card = await _context.Cards
                .FirstOrDefaultAsync(c => c.Id == cardId && c.UserID == userId);

            if (card == null)
            {
                return NotFound(new
                {
                    mesage = "Carta no encontrada o no te pertenece."
                });
            }

            if (card.Graded || card.GradeStatus == "accepted")
            {
                return BadRequest(new
                {
                    message = "Esta carta ya ha sido gradeada."
                });
            }

            if (card.GradeStatus == "requested")
            {
                return BadRequest(new
                {
                    message = "Ya has solicitado el gradeo de esta carta."
                });
            }

            // Si todo esta bien cambiamos el estado del gradeo
            card.GradeStatus = "requested";
            card.LastUpdatedAt = DateTime.Now;

            // Guardamos los cambios
            await _context.SaveChangesAsync();

			return Ok(new { message = "Gradeo solicitado correctamente." });

		}


		// ----------GetAllGradings----------

		// Método para que los admins y gradeadores rechacen un gradeo
		[Authorize(Roles = "admin, grader")] // Estos métodos son solo para los admins
        [HttpPost("reject")]
        public async Task<ActionResult> RejectGrading(int cardId)
        {
            // Buscamos la carta que se quiere rechazar
            var card = await _context.Cards.FindAsync(cardId);

            if (card == null)
            {
                return NotFound(new
                {
                    message = "Carta no encontrada."
                });
            }

            if (card.Graded || card.GradeStatus == "accepted" || card.GradeStatus != "requested")
            {
                return BadRequest(new
                {
                    message = "La carta no está en esta pendiente de gradeo."
                });
            }

            // Si todo va bien rechazamos el gradeo
            card.GradeStatus = "rejected";
            card.LastUpdatedAt= DateTime.Now;

            // Guardamos los cambios
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Gradeo rechazado correctamente."
            });
        }


		// ----------GetAllGradings----------

		// Método para obtener todos los gradeos 
		[Authorize(Roles = "admin, grader")] // Estos métodos son solo para los admins
		[HttpGet]
        public async Task<ActionResult> GetAllGradings(
            int page = 1,
			string? pokemonType = null,
			string? collection = null,
			string? pokemonName = null,
			string? rarity = null,
			string? orderBy = null,
            string? gradedBy = null
			)
        {
            int pageSize = 10;

            if (page < 1) page = 1;

            // Hacemos la consulta para obtener los gradeos de la base de datos
            var query = _context.Gradings
                .Include(g => g.Card)
                .Include(g => g.GradedBy)
                .AsQueryable();

            // Aplicamos los filtros y el orden
            query = ApplyFilters(query, pokemonType, collection, pokemonName, rarity, gradedBy);

            // Si no hay ningun tipo de resultado
            if (!await query.AnyAsync())
            {
                return NotFound(new
                {
                    message = "No se han encontrado gradeos con esos filtros."
                });
            }

            // Aplicamos la paginación y el orden
            var response = ApplyPagination(query, page, pageSize, orderBy);

            return Ok(response);
        }


		// ----------GetGradingById----------

		// Método para obtener un gradeo por su ID
		[Authorize(Roles = "admin, grader")] // Estos métodos son solo para los admins
		[HttpGet("{id}")]
        public async Task<ActionResult> GetGradingByID(int id)
        {
            var grading = await _context.Gradings
                .Include(g => g.Card)
                .Include(g => g.GradedBy)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grading == null)
            {
                return NotFound(new
                {
                    message = "Gradeo no encontrado."
                });
            }

            var response = MapToGradingResponse(grading);

            return Ok(response);
        }


		// ----------Delete----------

		// Método para eliminar un gradeo
		[Authorize(Roles = "admin, grader")] // Estos métodos son solo para los admins
		[HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGrading(int id)
        {
            var grading = await _context.Gradings.FindAsync(id);

            if (grading == null)
            {
                return NotFound(new
                {
                    message = "Gradeo no encontrado."
                });
            }

            // Cambiamos los datos de gradeo de la carta seleccionada
            var card = await _context.Cards.FindAsync(grading.CardId);

            if (card != null)
            {
                card.Graded = false;
                card.Grade = 0;
                card.LastUpdatedAt = DateTime.Now;
            }

            // Lo quitamos de la tabla de gradings
            _context.Gradings.Remove(grading);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Gradeo eliminado correctamente."
            });

        }


        // ----------Funciones----------

        // Funcion para mapear la respuesta de GradingResponse
        private GradingResponse MapToGradingResponse(Grading grading)
        {
            return new GradingResponse
            {
                Id = grading.Id,
                CardId = grading.CardId,
                PokemonName = grading.Card != null ? grading.Card.PokemonName : string.Empty,
                Collection = grading.Card != null ? grading.Card.Collection : string.Empty,
                Rarity = grading.Card != null ? grading.Card.Rarity : string.Empty,
                ImageUrl = grading.Card != null ? grading.Card.ImageUrl : string.Empty,
                Price = grading.Card != null ? grading.Card.Price : 0,

                Grade = grading.Grade,
                GradedByUsername = grading.GradedBy != null ? grading.GradedBy.Username : string.Empty,
            };
        }

		// Funcion para aplicar los filtros a las cartas
		private IQueryable<Grading> ApplyFilters(
			IQueryable<Grading> query,
			string? pokemonType,
			string? collection,
			string? pokemonName,
			string? rarity,
            string? gradedBy
			)
		{
			// Primer filtro, filtrar por el tipo del pokemon
			if (!string.IsNullOrEmpty(pokemonType))
			{
				query = query.Where(g => g.Card.PokemonType.ToLower().Contains(pokemonType.ToLower()));
			}

			// Filtrar por la coleccion de la carta
			if (!string.IsNullOrEmpty(collection))
			{
				query = query.Where(g => g.Card.Collection.ToLower().Contains(collection.ToLower()));
			}

			// Filtrar por el nombre del Pokémon
			if (!string.IsNullOrEmpty(pokemonName))
			{
				query = query.Where(g => g.Card.PokemonName.ToLower().Contains(pokemonName.ToLower()));
			}

			// Filtro por la rareza de la carta 
			if (!string.IsNullOrEmpty(rarity))
			{
				query = query.Where(g => g.Card.Rarity.ToLower().Contains(rarity.ToLower()));
			}

            // Filtro por el usuario que ha gradeado la carta
            if (!string.IsNullOrEmpty(gradedBy))
            {
                query = query.Where(g => g.GradedBy.Username.ToLower().Contains(gradedBy.ToLower()));
            }

			return query;
		}

        // Función para ordenar los gradeos
		private List<Grading> ApplyOrdering(List<Grading> gradings, string? orderBy)
		{
            // Comprobamos si el parámetro tiene valor
            if (!string.IsNullOrEmpty(orderBy))
            {
                // Hacemos un switch para todos los tipos de ordenacion
                orderBy = orderBy.ToLower();

                switch (orderBy)
                {
                    case "fecha-asc":
                        return gradings.OrderBy(g => g.GradedAt).ToList();

                    case "fecha-desc":
                        return gradings.OrderByDescending(g => g.GradedAt).ToList();

                    case "nombre":
                        return gradings.OrderBy(g => g.Card.PokemonName).ToList();

                    case "tipo":
                        return gradings.OrderBy(g => GetPokemonTypeOrder(g.Card.PokemonType)).ToList();

                    case "rareza":
                        return gradings.OrderBy(g => GetRarityOrder(g.Card.Rarity)).ToList();

                    case "grader":
                        return gradings.OrderBy(g => g.GradedBy.Username.ToLower()).ToList();

                    case "grader-desc":
                        return gradings.OrderByDescending(g => g.GradedBy.Username.ToLower()).ToList();

                    default:
                        return gradings.OrderByDescending(g => g.GradedAt).ToList();
                }
            } else
            {
				return gradings.OrderByDescending(g => g.GradedAt).ToList();
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

        // Función para paginar los gradeos
		private async Task<PagedResult<GradingResponse>> ApplyPagination(
			IQueryable<Grading> query, 
            int page, 
            int pageSize,
			string? orderBy = null
		)
		{
			// Obtenemos los gradings de la base de datos
			var gradings = await query
				.Include(g => g.Card)
				.Include(g => g.GradedBy)
				.ToListAsync();

			int totalCount = gradings.Count;
			int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

			// Ordenamos los grading
			gradings = ApplyOrdering(gradings, orderBy);

            // Ahora paginamos los gradings
            gradings = gradings.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			var items = gradings.Select(MapToGradingResponse).ToList();

			return new PagedResult<GradingResponse>
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
