namespace JwtSignFinal.Models
{
    public class ApiUsers
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? BirthPlace { get; set; }
        public byte IsEmailVerified { get; set; }
        public string? Token { get; set; }
        public DateTime ETokenExpiration { get; set; }
        public string? VerificationToken { get; set; }
    }
}
