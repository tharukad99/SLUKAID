namespace FloodRelief.Api.Dtos
{
    public class DonationCreateDto
    {
        public string DonorName { get; set; } = null!;
        public string? DonorEmail { get; set; }
        public string? DonorPhone { get; set; }
        public string ItemDescription { get; set; } = null!;
        public int? Quantity { get; set; }
        public decimal? WeightKg { get; set; }
        public int? CollectionPointId { get; set; }  // optional – for Admin
        public string? Notes { get; set; }
    }
}
