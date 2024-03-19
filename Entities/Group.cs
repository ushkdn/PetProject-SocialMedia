namespace SocialNetwork.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // public List<Post> Posts { get; set; } = [];
        public List<User> Followers { get; set; } = [];
        public List<User> JoinRequests { get; set; } = [];
        public bool IsClosed { get; set; }
    }
}