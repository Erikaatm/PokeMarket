using System.ComponentModel.DataAnnotations;

namespace PokeMarket.Models.DTOs
{
    public class UpdatePasswordRequest
    {
        [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La nueva contraseña tiene que tener mínimo 6 caracteres.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmación de la nueva contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La nueva contraseña tiene que tener mínimo 6 caracteres.")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
