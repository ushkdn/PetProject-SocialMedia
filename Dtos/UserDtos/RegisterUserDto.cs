using SocialNetwork.Entities;

namespace SocialNetwork.Dtos.UserDtos
{
    public class RegisterUserDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Surname { get; set; }
        public required string Name { get; set; }
        public required DateTime Birthday { get; set; }
        public required string Location { get; set; }
        
    }
}
