using System.ComponentModel.DataAnnotations;

namespace PokeMarket.Models.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico debe tener un formato válido.")]
        public string EmailAddress { get; set; } = string.Empty;

		[Required(ErrorMessage = "La confirmación del nuevo email es obligatoria.")]
		[EmailAddress(ErrorMessage = "La confirmación del nuevo email tiene que tener un formato válido.")]
		[Compare("EmailAddress", ErrorMessage = "Los emails no coinciden.")]
		public string ConfirmEmail { get; set; } = string.Empty;

		[Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña tiene que tener mínimo 6 caracteres.")]
        public string Password { get; set; } = string.Empty;

		[Required(ErrorMessage = "La confirmación de la nueva contraseña es obligatoria.")]
		[MinLength(6, ErrorMessage = "La confirmacón de la contraseña tiene que tener mínimo 6 caracteres.")]
		[Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
		public string ConfirmPassword { get; set; } = string.Empty;

	}
}
