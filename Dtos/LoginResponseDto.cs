namespace FloodRelief.Api.Dtos
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int? CollectionPointId { get; set; }
    }
}
