namespace FloodRelief.Api.Dtos
{
    public class CollectionPointDto
    {
        public int CollectionPointId { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? District { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
    }
}
