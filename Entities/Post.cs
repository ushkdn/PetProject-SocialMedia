namespace SocialNetwork.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public uint Likes { get; set; }
    }
}