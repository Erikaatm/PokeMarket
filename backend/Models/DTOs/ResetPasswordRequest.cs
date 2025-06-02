using System.ComponentModel.DataAnnotations;

namespace PokeMarket.Models.DTOs
{
	public class ResetPasswordRequest
	{
		[Required(ErrorMessage = "El token es obligatorio.")]
		public string Token { get; set; } = string.Empty;

		[Required(ErrorMessage = "La nueva contraseña es olbigarotia.")]
		[MinLength(6, ErrorMessage = "La nueva contraseña tiene que tener mínimo 6 caracteres.")]
		public string NewPassword { get; set; } = string.Empty;

		[Required(ErrorMessage = "La confirmación de la nueva contraseña es obligatoria.")]
		[MinLength(6, ErrorMessage = "La confirmación de la nueva contraseña tiene que tener mínimo 6 caracteres.")]
		[Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
		public string ConfirmNewPassword { get; set; } = string.Empty;
	}
}
