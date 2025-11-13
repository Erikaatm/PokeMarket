using System.ComponentModel.DataAnnotations;

namespace PokeMarket.Models.DTOs
{
    public class UpdateUserProfileRequest
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string UserName { get; set; } = string.Empty;

        [MaxLength(30, ErrorMessage = "Tiene que tener máximo 30 caracteres.")]
        public string? FirstName { get; set; }

        [MaxLength(30, ErrorMessage = "Tiene que tener máximo 30 caracteres.")]
        public string? LastName1 { get; set; }

        [MaxLength(30, ErrorMessage = "Tiene que tener máximo 30 caracteres.")]
        public string? LastName2 { get; set; }

        [Phone(ErrorMessage = "Debe ser un número de teléfono válido.")]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

		[Required(ErrorMessage = "El email actual es obligatorio.")]
		[EmailAddress(ErrorMessage = "El email actual tiene que tener un formato válido.")]
		public string CurrentEmail { get; set; } = string.Empty;

		[Required(ErrorMessage = "La contraseña es obligatoria.")]
		public string CurrentPassword { get; set; } = string.Empty;
	}
}
