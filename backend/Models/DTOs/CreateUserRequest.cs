using System.ComponentModel.DataAnnotations;

namespace PokeMarket.Models.DTOs
{
	public class CreateUserRequest
	{
		// Este DTO es para crear usuarios en el rol de administrador

		[Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
		public string Username { get; set; } = string.Empty;

		[Required(ErrorMessage = "El correo electrónico es obligatorio.")]
		[EmailAddress(ErrorMessage = "El correo electrónico debe tener un formato válido.")]
		public string EmailAddress { get; set; } = string.Empty;

		[Required(ErrorMessage = "La contraseña es obligatoria.")]
		[MinLength(6, ErrorMessage = "La contraseña tiene que tener mínimo 6 caracteres.")]
		public string Password { get; set; } = string.Empty;

		[Required(ErrorMessage = "El rol es obligatorio.")]
		public string Role { get; set; } = "user";
	}
}
