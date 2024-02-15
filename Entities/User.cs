namespace SocialNetwork.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Location { get; set; }
        public List<User> Friends { get; set; }
        public string Email { get; set; }
        public string SentSecurityCode {  get; set; }
        public string PasswordHash { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; } = DateTime.UtcNow;
        public DateTime TokenExpires { get; set; }
    }
}
