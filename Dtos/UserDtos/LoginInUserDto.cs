namespace SocialNetwork.Dtos.UserDtos
{
    public class LoginInUserDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
