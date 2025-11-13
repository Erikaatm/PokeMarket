using System.ComponentModel.DataAnnotations;

namespace PokeMarket.Models.DTOs
{
    public class ForgotPasswordRequest
    {
        // Este DTO recoge el email del usuario para pedir la recuperación de la contraseña
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ser un email válido.")]
        public string Email { get; set; } = string.Empty;

    }
}
