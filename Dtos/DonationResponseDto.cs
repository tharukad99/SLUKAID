namespace FloodRelief.Api.Dtos
{
    public class DonationResponseDto
    {
        public int DonationId { get; set; }
        public string DonorName { get; set; } = null!;
        public string? DonorEmail { get; set; }
        public string? DonorPhone { get; set; }
        public string ItemDescription { get; set; } = null!;
        public int? Quantity { get; set; }
        public decimal? WeightKg { get; set; }
        public int CollectionPointId { get; set; }
        public string CollectionPointName { get; set; } = null!;
        public int CollectedByUserId { get; set; }
        public string CollectedByName { get; set; } = null!;
        public DateTime CollectedAt { get; set; }
        public string? Notes { get; set; }
    }
}
