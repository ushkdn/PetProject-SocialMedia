﻿namespace SocialNetwork.Entities
{
    public class MetaData
    {
        [Key]
        public int OwnerId { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = false;
        public string PasswordHash { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}