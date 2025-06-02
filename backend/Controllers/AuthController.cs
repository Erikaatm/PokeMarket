using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;// NECESARIO para SecurityKey, JwtSecurityTokenHandler, etc
using System.IdentityModel.Tokens.Jwt; // NECESARIO para JwtSecurityToken
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography; 
using PokeMarket.Data;
using PokeMarket.Models;
using PokeMarket.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using PokeMarket.Services;



namespace PokeMarket.Controllers
{
    // Esta clase será para los usuarios, en el frontend
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Accedemos a la base de datos 
        private readonly ApplicationDbContext _context;

        // Esto nos permitirá leer datos del appsetings.json
        private readonly IConfiguration _configuration;

        // Accedemos al servicio Email
        private readonly EmailService _emailService;

        public AuthController(ApplicationDbContext context, IConfiguration configuration, EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
		}

        // ----------Registro-----------

        // Hacemos el método del registro para los usuarios
        [HttpPost("register")] // Enviamos un POST con URL -> /api/auth/register
        public async Task<ActionResult> Register(RegisterRequest request)
        {
			// Comprobamos que se realizan bien las validaciones de los DTOs
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Validamos si el email que hemos puesto ya está en uso o no
			var exists = await _context.Users.AnyAsync(u => u.EmailAddress == request.EmailAddress);

            if (exists)
            {
                return BadRequest(new {
                    message = "El email ya está registrado."
                });
            }

            // Si el email no está registrado
            // Creamos un usuario nuevo
            string role = "user";

            if (request.EmailAddress.EndsWith("@PokeMarket.com"))
            {
                role = "grader";

            } else if (request.EmailAddress.EndsWith("@AdminPokeMarket.com"))
            {
                role = "admin";
            }

            var user = new User
            {
                Username = request.Username,
                EmailAddress = request.EmailAddress,
                Role = role,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now,

            };

            // Ahora generamos el token para la verificación del email
            var emailToken = GenerateRandomToken();

            // Le damos 24 horas de tiempo para que este expire
            var tokenExpiration = DateTime.Now.AddHours(24);

            // Añadimos el token y su expiracion al usuario
            user.EmailConfirmationToken = emailToken;
            user.EmailConfirmationTokenExpiresAt = tokenExpiration;

            // Ahora hasheamos la contraseña para que sea seguro
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            // Añadimos a la base de datos y guardamos los cambios
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Ahora usaremos el EmailService para enviar un enlace de verificación de correo cuando una persona se registre
            // Crearemos el enlace de confirmación 
            var confirmationLink = $"http://localhost:4200/confirm-email?token={user.EmailConfirmationToken}";

            // Ahora tenemkos que rellenar el asunto y el contenido del email
            string subject = "Confirme su cuenta de PokeMarket.";
            string body = $"<p>¡Hola {user.Username}!, </p>" +
                          $"<p>Gracias por registrarte en PokeMarket.</p>" +
                          $"<p>Para continuar, haz click en el siguiente enlace para verificar su correo electrónico:</p>" +
                          $"<p><a href='{confirmationLink}'>Verificar email</a></p>" +
                          $"<p>Este enlace expirará en las próximas 24 horas.</p>";

            // Llamamos al servicio y enviamos el email con los parametros que hemos rellenado antes
            await _emailService.SendEmailAsync(user.EmailAddress, subject, body);

			return Ok(new
            {
                message = "Usuario registrado correctamente. Revise su correo para verificar su cuenta y completar el registro."
            });
        }


        // ----------ConfirmacionEmail-----------
        // Este método es el encargado de enviar el email de confirmacion por medio del token que se le pasa por parámetro
        [HttpGet("confirm-email")]
        public async Task<ActionResult> ConfirmEmail(string token)
        {
            // Buscamos al usuario que tenga el token que hemos pasado por parametro
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);

            // Si no lo encuentra
            if (user == null)
            {
                return NotFound(new
                {
                    message = "Usuario no encontrado o token inválido."
                });
            }

            // Comprobamos si el token ha expirado
            if (user.EmailConfirmationTokenExpiresAt < DateTime.UtcNow)
            {
                return BadRequest(new
                {
                    message = "El enlace de verificación ha expirado. Solicita uno nuevo."
                });
            }

            // Ahora tenemos que marcar el email como verificado y limpiar el token para que no se pueda volver a usar
            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpiresAt = null;

            // Actualizamos la fecha de modificacion
            user.LastUpdatedAt = DateTime.UtcNow;

            // Guardamos los cambios
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Correo electrónico verificado correctamente."
            });

        }

        // Método para reenviar el correo de verificar el email
        [HttpPost("resend-confirm-email")]
        public async Task<ActionResult> ResendConfirmEmail(ResendConfirmEmailRequest request)
        {
			// Comprobamos que se realizan bien las validaciones de los DTOs
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

            // Validamos si se ha escrito el email
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new
                {
                    message = "El email es obligatorio."
                });
            }

            // Buscamos al usuario por el mail facilitado
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == request.Email);

            // Comprobamos si encontramos al usuario
            if (user == null)
            {
                return NotFound(new
                {
                    message = "No hay ningún usuario registrado con el email facilitado."
                });
            }

            // Comprobamos si el email ya ha sido confirmado antertiormente
            if (user.IsEmailConfirmed)
            {
                return BadRequest(new
                {
                    message = "El email de este usuario ya ha sido verificado anteriormente."
                });
            }

            // Si todo va bien, generamos un nuevo token y una fecha de expiración de este
            user.EmailConfirmationToken = GenerateRandomToken();
            user.EmailConfirmationTokenExpiresAt = DateTime.UtcNow.AddHours(24);
            user.LastUpdatedAt = DateTime.UtcNow;

            // Guardamos los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Ahora creamos el enlace de verificación del correo
            var confirmationLink = $"http://localhost:4200/confirm-email?token={user.EmailConfirmationToken}";

            // Rellenamos el asunto y el cuerpo del email
            string subject = "Reenvío de verificación del correo electrónico - PokeMarket.";
            string body = $"<p>¡Hola {user.Username}!, </p>" +
						  $"<p>Nos ha solicitado un nuevo enlace para verificar su correo electrónico.</p>" +
						  $"<p>Para continuar, haz click en el siguiente enlace para verificar su correo electrónico:</p>" +
						  $"<p><a href='{confirmationLink}'>Verificar email</a></p>" +
						  $"<p>Este enlace expirará en las próximas 24 horas.</p>";

            // Enviamos el correo 
            await _emailService.SendEmailAsync(user.EmailAddress, subject, body);

			// Y se envia el email con el enlace de verificación
			return Ok(new
            {
                message = "Email enviado reenviado correctamente. Revise su bandeja de entrada.",
            });

		}


        // Método para recuperar la contraseña
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            // Comprobamos que se realizan las validaciones del DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Buscamos al usuario por el email que ha facilitado
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == request.Email); 

            // Comprobamos si el user es null
            if (user == null)
            {
                  return NotFound(new
                  {
                      message = "No hay ningún usuario con ese correo electrónico."
                  });
            }

            // Si el correo no ha sido verificado no puede recuperar la contraseña
            if (!user.IsEmailConfirmed)
            {
                return BadRequest(new
                {
                    message = "Debes verificar el correo electrónico para solicitar una nueva contraseña."
                });
            }

            // Generamos un token para cambiar la contraseña
            user.PasswordResetToken = GenerateRandomToken();
            user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);
            user.LastUpdatedAt = DateTime.UtcNow;

            // Guardamos los cambios
            await _context.SaveChangesAsync();

            // Creamos el enlace para que el usuario cambie la contraseña
            var resetPasswordLink = $"http://localhost:4200/reset-password?token={user.PasswordResetToken}";

            // Rellenamos el email de recuperación de contraseña
            var subject = "Recuperar contraseña - PokeMarket";
            var body = $"<p>¡Hola {user.Username}!, </p>" +
					   $"<p>Aqui tienes un link para restablecer tu contraseña.</p>" +
					   $"<p>Para continuar, haz click en el siguiente enlace:</p>" +
					   $"<p><a href='{resetPasswordLink}'>Recuperar contraseña</a></p>" +
					   $"<p>Este enlace expirará en las próxima hora.</p>" +
					   $"<p>Si no fuiste tú, ignora este mensaje.</p>";

            // Mandamos el email
            await _emailService.SendEmailAsync(user.EmailAddress, subject , body);

            return Ok(new
            {
                message = "Se ha enviado un correo para recuperar la contraseña. Revise su bandeja de entrada."
            });

		}


        // Método para resetear la contraseña a partir del link de recuperación de contraseña del correo
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
        {
            // Comprobamos que las validaciones se realizan correctamente
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Buscamos al usuario con el token del link que uso el usuario
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);

            // Verificamos si el usuario es nulo
            if (user == null)
            {
                return NotFound(new
                {
                    message = "Link inválido o ya utilizado. Vuelva a solicitar un link de recuperación de contraseña."
                });
            }

            // Verificamos si el token ha expirado
            if (user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
            {
                return BadRequest(new
                {
                    message = "El link ha expirado. Vuelva a solicitar un link de recuperación de contraseña."
				});
            }

            // Comprobamos que la contraseña nueva no sea igual a la actual
            var passwordHasher = new PasswordHasher<User>();
            var verifyPassword = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.NewPassword);

            if (verifyPassword == PasswordVerificationResult.Success)
            {
                return BadRequest(new
                {
                    message = "La contraseña nueva no puede ser igual a la actual."
                });
            }

            // Si todo está bien actualizamos la contraseña
            user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);

            // Limpiamos el token del link
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiresAt = null;
            user.LastUpdatedAt = DateTime.UtcNow;

            // Guardamos los cambios
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Contraseña actualizada correctamente. Ya puede iniciar sesión."
            });

        }


		// ----------Login-----------

		// Método para logearse
		[HttpPost("login")]
        // Se cogen los datos del body de la petión HTTP y se usan para construir el objeto LoginRequest
        public async Task<ActionResult> Login(LoginRequest request)
        {
			// Comprobamos que se realizan bien las validaciones de los DTOs
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Buscamos al usuario por el email en la base de datos
			var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == request.Email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Email o contraseña incorrectos."
                });
            }

            // Comprobamos si el email ha sido verificado
            if (!user.IsEmailConfirmed)
            {
                return Unauthorized(new
                {
                    message = "Debes verificar tu correo electrónico antes de iniciar sesión."
                });
            }

            // Verificamos la contraseña con el hasher
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            // Si no son iguales
            if(result != PasswordVerificationResult.Success)
            {
                return Unauthorized(new
                {
                    message = "Email o contraseña incorrectos."
                });
            }

            // Ahora vamos a crear los claims, que son datos del usuario que van dentro del token JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.Role, user.Role),
                // Así podemos ver quien ha hecho la peticion
            };

			// Creamos una clave secreta desde appsettings.json
            // Aqui estamos verificando que la key no sea nula
			var jwtKey = _configuration["Jwt:Key"];
			if (string.IsNullOrEmpty(jwtKey))
			{
				return StatusCode(500, new { message = "Error interno: No se encontró la clave JWT en la configuración." });
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Se firma el token con mi clave

            // Definimos el contenido del token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // Quién ha creado el token
                audience: _configuration["Jwt:Audience"], // Quién lo debería usar 
                claims: claims, // Lo que metes dentro del token
                expires: DateTime.Now.AddHours(3), // Cuanto tiempo dura
                signingCredentials: creds // Como y con que clave se firma
            );

            // Convertimos el objeto JwtSecurityToken en un string que podemos mandar al frontend
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            // Devolvemos mensaje de éxito
            return Ok(new
            {
                message = "Inicio de sesión exitoso.",
                token = jwt,
                user = new
                {
                    user.Id,
                    user.Username,
                    user.EmailAddress
                }
            });


        }


        // ----------VerUsuarioActual-----------

        // Método para ver el perfil del usuario logeado
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult> GetCurrentUser()
        {
            // Obtenemos el ID del usuario que se ha logeado
            var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            // Comprobamos si existe el usuario
            if (getUserId == null)
            {
                return NotFound(new
                {
                    message = "Debes iniciar sesión para ver tu perfil."
                });
            }

            // Guardamos el id en una varaible
            int userId = int.Parse(getUserId.Value);

            // Buscamos al usuario en la base de datos
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "Usuario no encontrado."
                });
            }

            return Ok(new
            {
                user.Id,
                user.Username,
                user.EmailAddress,
                user.Role,
                user.CreatedAt,
                user.LastUpdatedAt

            });
        }


        // ----------UpdateUser-----------

        // Método para actualizar el perfil del usuario logeado
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<ActionResult> UpdateProfile(UpdateUserProfileRequest request)
        {
			// Comprobamos que se realizan bien las validaciones de los DTOs
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Obtenemos el id del usuario logeado
			var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if (getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para actualizar tu perfil."
                });
            }

            int userId = int.Parse(getUserId.Value);

            // Buscamos el usuario en la base de datos
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "Usuario no encontrado."
                });
            }

            // Actualizamos los datos, solo si estos están rellenos
            user.Username = request.UserName;
            user.FirstName = !string.IsNullOrEmpty(request.FirstName) ? request.FirstName : user.FirstName;
            user.LastName1 = !string.IsNullOrEmpty(request.LastName1) ? request.LastName1 : user.LastName1;
            user.LastName2 = !string.IsNullOrEmpty(request.LastName2) ? request.LastName2 : user.LastName2;
            user.PhoneNumber = !string.IsNullOrEmpty(request.PhoneNumber) ? request.PhoneNumber : user.PhoneNumber;
            user.Address = !string.IsNullOrEmpty(request.Address) ? request.Address : user.Address;
            user.LastUpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Usuario actualizado correctamente.",
                user = new
                {
                    user.Id,
                    user.Username,
                    user.EmailAddress,
                    user.FirstName,
                    user.LastName1,
                    user.LastName2,
                    user.PhoneNumber,
                    user.Address,
                    user.Role,
                    user.CreatedAt,
                    user.LastUpdatedAt
                }
            });

        }


        // ----------UpdateEmail-----------

        // Método para cambiar el email del usuario
        [Authorize]
        [HttpPut("update-email")]
        public async Task<ActionResult> UpdateEmail(UpdateEmailRequest request)
        {
			// Comprobamos que se realizan bien las validaciones de los DTOs
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Obtenemos el id del usuario 
			var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);

            if (getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para actualizar tu email."
                });
            }

            int userId = int.Parse(getUserId.Value);

            // Verificamos si el usuario existe
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "Usuario no encontrado."
                });
            }

            // Comprobamos que el email actual coincide
            if(!string.Equals(user.EmailAddress, request.CurrentEmail, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = "El email actual no coincide con el introducido en la página."
                });
            }

            // Verificamos la contraseña 
            var passwordHasher = new PasswordHasher<User>();
            var verifyPassword = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);

            if(verifyPassword != PasswordVerificationResult.Success)
            {
                return Unauthorized(new
                {
                    message = "La contraseña es incorrecta."
                });
            }

            // Comprobamos que el nuevo email no esta ya en uso en nuestra base de datos
            var emailExists = await _context.Users.AnyAsync(u => u.EmailAddress == request.NewEmail);

            if (emailExists)
            {
                return BadRequest(new
                {
                    message = "El nuevo email que has introducido ya esta en uso."
                });
            }

            // Si todo está en orden
            // Actualizamos el email por el nuevo que introdujo el usuario
            user.EmailAddress = request.NewEmail;
            user.LastUpdatedAt = DateTime.Now;

            // Guardamos los cambios
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Email actualizado correctamente.",
                user = new
                {
                    user.Id,
                    user.Username,
                    user.EmailAddress,
                    user.LastUpdatedAt
                }
            });
        }


		// ----------UpdatePassword-----------

		// Método para actualizar la contraseña
		[Authorize]
        [HttpPut("update-password")]
        public async Task<ActionResult> UpdatePassword(UpdatePasswordRequest request)
        {
			// Comprobamos que se realizan bien las validaciones de los DTOs
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Obtenemos el id del usuario
			var getUserId = User.FindFirst(ClaimTypes.NameIdentifier);
            
            if (getUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión para actualizar tu contraseña."
                });
            }

            // Guardamos el id
            int userId = int.Parse(getUserId.Value);

            // Comprobamos si el user existe
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                    return NotFound(new
                    {
                        message = "Usuario no encontrado."
                    });
            }

            // Verificamos si la contraseña actual que hemos introducido es correcta
            var passwordHasher = new PasswordHasher<User>();
            var verifyPassword = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);

            if(verifyPassword != PasswordVerificationResult.Success)
            {
                return Unauthorized(new
                {
                    message = "La contraseña actual es incorrecta."
                });
            }

            // Comprobamos si las contraseñas son iguales
            if (request.NewPassword == request.CurrentPassword)
            {
                return BadRequest(new { message = "La nueva contraseña no puede ser igual a la actual." });
            }

            // Si todo va bien entonces
            // Hasheamos la nueva contraseña que hemos introducido
            user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
            user.LastUpdatedAt = DateTime.Now;

            // Guardamos los cambios
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Contraseña actualizada correctamente."
            });

        }


		// ----------Funciones-----------

        // Función para generar un token aleatorio para la verificación del email
        private string GenerateRandomToken()
        {
            // Aqui estamos creando un array de 32 bytes
            var tokenBytes = new byte[32];

            // Luego usamos un generador aleatorio seguro para rellenar ese array con numeros aleatorios
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }

            // Y por ultimo convertimos ese array en un string en formato base64, el cual es más fácil de usar en las URLs
            return Convert.ToBase64String(tokenBytes);
        }

	}
}
