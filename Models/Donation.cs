namespace FloodRelief.Api.Models
{
    public class Donation
    {
        public int DonationId { get; set; }
        public string DonorName { get; set; } = null!;
        public string? DonorEmail { get; set; }
        public string? DonorPhone { get; set; }
        public string ItemDescription { get; set; } = null!;
        public int? Quantity { get; set; }
        public decimal? WeightKg { get; set; }
        public int CollectionPointId { get; set; }
        public int CollectedByUserId { get; set; }
        public DateTime CollectedAt { get; set; }
        public string? Notes { get; set; }

        public CollectionPoint CollectionPoint { get; set; } = null!;
        public User CollectedByUser { get; set; } = null!;
    }
}
