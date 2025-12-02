namespace FloodRelief.Api.Models
{
    public class CollectionPoint
    {
        public int CollectionPointId { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? District { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
    }
}
