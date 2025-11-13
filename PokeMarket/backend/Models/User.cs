using System.ComponentModel.DataAnnotations;

namespace PokeMarket.Models
{
    public class User
    {
        // Id del usuario
        public int Id { get; set; }

        // Nombre de usuario
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [MaxLength(30)]
        public string Username { get; set; } = string.Empty;

        // Nombre del usuario
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(30)]
        public string FirstName { get; set; } = string.Empty;

        // Primer apellido del usuario
        [Required(ErrorMessage = "El primer apellido es obligatorio.")]
        [MaxLength(30)]
        public string LastName1 { get; set; } = string.Empty;

		// Segundo apellido del usuario
		[MaxLength(30)]
		public string? LastName2 { get; set; }

        // Email del usuario
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ser un email válido.")]
        public string EmailAddress { get; set; } = string.Empty;

        // Indica si el usuario a verificado su email
        public bool IsEmailConfirmed { get; set; } = false;

        // Token generado para verificar el email (este puede ser null hasta que se genere)
        public string? EmailConfirmationToken { get; set; }

        // La fecha en la que expira el token de verificación del email
        public DateTime? EmailConfirmationTokenExpiresAt { get; set; }

        // Contraseña del usuario
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string PasswordHash {  get; set; } = string.Empty;

        // Token para recuperar la contraseña 
        public string? PasswordResetToken { get; set; }

        // Fecha en la que expira el token
        public DateTime? PasswordResetTokenExpiresAt { get; set; }

        // Telefono del usuario
        [Phone(ErrorMessage = "Debe ser un teléfono válido.")]
        public string? PhoneNumber {  get; set; }

        // Dirección del usuario
        public string? Address {  get; set; }

        // Fecha de creación
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Fecha de modificación
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;

        // Roles de los usuarios
        public string Role { get; set; } = "user";
    }
}
