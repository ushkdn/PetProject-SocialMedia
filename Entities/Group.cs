namespace SocialNetwork.Entities
{
    public class Group
    {
        public string Id { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Post> Posts { get; set; } = [];
        public List<UserGroups> Followers { get; set; } = [];
        public List<UserJoinRequest> JoinRequests { get; set; } = [];
        public bool IsClosed { get; set; }
    }
}