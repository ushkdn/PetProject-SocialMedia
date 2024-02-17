namespace SocialNetwork.Entities
{
    public class MetaData
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string SecurityCode { get; set; } = string.Empty;
        public DateTime SecurityCodeCreated { get; set; }
        public DateTime SecurityCodeExprires { get; set; }
        public bool IsVerified { get; set; } = false;
        public string PasswordHash { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}
