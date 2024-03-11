namespace SocialNetwork.Dtos.UserDtos
{
    public class ResetPasswordDto
    {
        [Required]
        public string Code { get; set; }

        [Required, MinLength(7)]
        public string Password { get; set; }

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}