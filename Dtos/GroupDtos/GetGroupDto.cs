namespace SocialNetwork.Dtos.GroupDtos
{
    public class GetGroupDto
    {
        public int Id { get; set; }
        public int GroupOwnerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Post> Posts { get; set; }
        public List<User> Followers { get; set; }
        public bool IsClosed { get; set; }
    }
}