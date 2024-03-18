namespace SocialNetwork.Dtos.UserDtos
{
    public class GetUserDto
    {
        public string Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Location { get; set; }
        public List<User> Friends { get; set; }
    }
}