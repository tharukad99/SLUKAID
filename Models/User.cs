namespace FloodRelief.Api.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int? CollectionPointId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public CollectionPoint? CollectionPoint { get; set; }
    }
}
