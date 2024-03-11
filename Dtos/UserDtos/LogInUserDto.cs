namespace SocialNetwork.Dtos.UserDtos
{
    public class LogInUserDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
