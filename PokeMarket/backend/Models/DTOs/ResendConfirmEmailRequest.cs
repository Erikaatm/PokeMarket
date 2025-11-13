using System.ComponentModel.DataAnnotations;

namespace PokeMarket.Models.DTOs
{
	public class ResendConfirmEmailRequest
	{
		// Este DTO sirve para introducir el email para volver a enviar el enlace de verificación
		[Required(ErrorMessage = "El email es olbigatorio.")]
		[EmailAddress(ErrorMessage = "Debe ser un email válido.")]
		public string Email { get; set; } = string.Empty;

	}
}
