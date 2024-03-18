namespace SocialNetwork.Entities
{
    public class Post
    {
        public string Id { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public uint Likes { get; set; }
    }
}