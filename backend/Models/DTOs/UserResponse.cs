namespace PokeMarket.Models.DTOs
{
    public class UserResponse
    {
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string FirstName {  get; set; } = string.Empty;

        public string LastName1 { get; set; } = string.Empty;

        public string? LastName2 { get; set; }

        public string EmailAddress { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; } = string.Empty;

        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public string Role { get; set; } = "user";

    }
}
