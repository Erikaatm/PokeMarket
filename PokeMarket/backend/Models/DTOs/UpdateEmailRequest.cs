using System.ComponentModel.DataAnnotations;

namespace PokeMarket.Models.DTOs
{
    public class UpdateEmailRequest
    {
        [Required(ErrorMessage = "El email actual es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email actual tiene que tener un formato válido.")]
        public string CurrentEmail { get; set; } = string.Empty;


        [Required(ErrorMessage = "El nuevo email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El nuevo email tiene que tener un formato válido.")]
        public string NewEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmación del nuevo email es obligatoria.")]
        [EmailAddress(ErrorMessage = "La confirmación del nuevo email tiene que tener un formato válido.")]
        [Compare("NewEmail", ErrorMessage = "Los emails no coinciden.")]
        public string ConfirmNewEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string CurrentPassword {  get; set; } = string.Empty;

    }
}
