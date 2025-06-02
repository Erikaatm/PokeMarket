using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokeMarket.Data;
using PokeMarket.Models;
using PokeMarket.Models.DTOs;

namespace PokeMarket.Controllers
{
    // Esta clase será para los administradores, los cuales podrán administrar todo sobre los usuarios, en un panel de control
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // Conectamos con la base de datos
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

		// ----------Create----------

		// Metodo para crear un usuario
		[HttpPost]
        public async Task<ActionResult> CreateUser(CreateUserRequest request)
        {
            // Comprobamos si las validaciones funcionan correctamente
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			// Validamos que el rol introducido es válido
			var validRoles = new List<string> { "user", "grader", "admin" };

			if (!validRoles.Contains(request.Role.ToLower()))
			{
				return BadRequest(new
				{
					message = "El rol introducido no es válido. Los valores permitidos son: user, grader o admin."
				});
			}

			// Comprobamos si el email esta en uso
			var exists = await _context.Users
                .AnyAsync(u => u.EmailAddress == request.EmailAddress);

            if (exists)
            {
                return BadRequest(new
                {
                    message = "El email ya está en uso."
                });
            }

            // Comprobamos si el nombre de usuario ya está en uso
            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username == request.Username);

            if (usernameExists)
            {
                return BadRequest(new
                {
                    message = "El nombre de usuario ya está en uso."
                });
            }

            // Creamos el usuario
            var user = new User
            {
                Username = request.Username,
                EmailAddress = request.EmailAddress,
                Role = request.Role,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now,
            };

            // Hasheamos la contraseña
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            // Añadimos el usuario creado por el admin a la base de datos
            _context.Users.Add(user);

            // Guardamos los cambios
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Usuario creado correctamente."
            });

        }


		// ----------GetAllUsers----------

		// Método para obtener todos los usuarios
		[HttpGet]
        public async Task<ActionResult> GetUsers(
            int page = 1,
            string? username = null,
            string? email = null,
            string? role = null,
            string? orderBy = null
            )
        {
            if (page < 1) page = 1;

            int pageSize = 10;

            // Obtenemos todos los users que se han creado
            var query = _context.Users.AsQueryable();

            // Aplicamos los filtros y la ordenacion
            query = ApplyUserFilters(query, username, email, role);
            query = ApplyUserOrdering(query, orderBy);

            // Comprobamos si está vacío
            if (!await query.AnyAsync())
            {
                return NotFound(new
                {
                    message = "No se han encontrado usuarios con esos filtros."
                });
            }

            // Aplicamos la paginacion
             var result = await ApplyUserPagination(query, page, pageSize);

            return Ok(result);
        }

        // Método para obtener un obtener un usuario por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserByID(int id)
        {
            // Guardamos al usuario que obtenemod con el id al buscar en la base de datos
            var selectedUser = await _context.Users.FindAsync(id);

            // Comprobamos si existe
            if (selectedUser == null)
            {
                return NotFound(new
                {
                    message = "Usuario no encontrado."
                });
            }

            var response = MapToUserRespose(selectedUser);

            return Ok(new
            {
                user = response
            });
        }


		// ----------Update----------

		// Método para actualizar usuarios
		[HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, UpdateUserProfileRequest request)
        {
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Buscamos al usuario en la base de datos 
			var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "Usuario no encontrado."
                });
            }

			// Comprobamos si el nombre de usuario ya está en uso
			bool usernameExists = await _context.Users
				.AnyAsync(u => u.Username == request.UserName && u.Id != id);

			if (usernameExists)
			{
				return BadRequest(new
				{
					message = "El nombre de usuario ya está en uso."
				});
			}

			// Actualizamos los datos que esten con algun valor
			user.Username = request.UserName;
            user.FirstName = !string.IsNullOrEmpty(request.FirstName) ? request.FirstName : user.FirstName;
            user.LastName1 = !string.IsNullOrEmpty(request.LastName1) ? request.LastName1 : user.LastName1;
            user.LastName2 = !string.IsNullOrEmpty(request.LastName2) ? request.LastName2 : user.LastName2;
            user.PhoneNumber = !string.IsNullOrEmpty(request.PhoneNumber) ? request.PhoneNumber : user.PhoneNumber;
            user.Address = !string.IsNullOrEmpty(request.Address) ? request.Address : user.Address;

            user.LastUpdatedAt = DateTime.Now;
            // Guardamos los cambios
            await _context.SaveChangesAsync();

            // Devolvemos un mensaje de existo
            return Ok(new
            {
                message = "Usuario actualizado correctamente."
            });
        }


		// ----------Delete----------

		// Método para eliminar un usuario por su ID
		[HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            // Buscamos el usuario por su ID
            var user = await _context.Users.FindAsync(id);

            // Si no existe, devolvemos error 404
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Eliminamos el usuario
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            var response = MapToUserRespose(user);

            return Ok(new
            {
                message = "Usuario eliminado correctamente.",
                user = response
            });
        }


		// ----------Funciones----------

        // Funcion para mapear UserResponse
        private UserResponse MapToUserRespose (User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                UserName = user.Username,
                FirstName = user.FirstName,
                LastName1 = user.LastName1,
                LastName2 = user.LastName2,
                EmailAddress = user.EmailAddress,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                CreatedAt = user.CreatedAt,
                LastUpdatedAt = user.LastUpdatedAt,
                Role = user.Role,
            };
        }

		// Funcion para aplicar el paginado
		private async Task<PagedResult<UserResponse>> ApplyUserPagination(IQueryable<User> query, int page, int pageSize)
		{
			// Contamos cuantas cartas totales hay tras aplicar los filtros correspondientes y la ordenación
			var totalCount = await query.CountAsync();

			// Calculamos el número total de páginas 
			var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

			// Aplicamos paginación
			var users = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			// Mapeamos la lista de trades con la funcion de CardResponse
			var items = users.Select(MapToUserRespose).ToList();

			// Y devolvemos el resultado 
			return new PagedResult<UserResponse>
			{
				CurrentPage = page,
				PageSize = pageSize,
				TotalPages = totalPages,
				TotalCount = totalCount,
				Items = items

			};
		}

        // Funcion para aplicar orden en los usuarios
        private IQueryable<User> ApplyUserOrdering(IQueryable<User> query, string? orderBy)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                return query.OrderByDescending(u => u.CreatedAt);
            }

            orderBy = orderBy.ToLower();

            switch (orderBy)
            {
                case "fecha-asc":
                    return query.OrderBy(u => u.CreatedAt);

                case "fecha-desc":
                    return query.OrderByDescending(u => u.CreatedAt);

                case "nombre-asc": 
                    return query.OrderBy(u => u.Username);

                case "nombre-desc": 
                    return query.OrderByDescending(u => u.Username);

                case "rol-asc":
                    return query.OrderBy(u => u.Role);

                case "rol-desc":
                    return query.OrderByDescending(u => u.Role);

                default:
					return query.OrderByDescending(u => u.CreatedAt);

			}
		}

        // Funcion para filtrar los usuarios
        private IQueryable<User> ApplyUserFilters(
            IQueryable<User> query,
            string? username,
            string? email,
            string? role
            )
        {
            // Filtramos por nombre de usuario
            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(u => u.Username.ToLower().Contains(username.ToLower()));
            }

            // Filtramos por email del usuario
            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.EmailAddress.ToLower().Contains(email.ToLower()));
            }

            // Filtramos por el rol del usuario
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role.ToLower().Contains(role.ToLower()));
            }

            return query;
        }
	}
}

