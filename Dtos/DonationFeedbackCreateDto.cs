namespace FloodRelief.Api.Dtos
{
    public class DonationFeedbackCreateDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }

        public string AddressLine1 { get; set; } = null!;
        public string? AddressLine2 { get; set; }

        public string Postcode { get; set; } = null!;
        public string ItemsDescription { get; set; } = null!;
    }
}
