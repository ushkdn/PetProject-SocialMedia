namespace SocialNetwork.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Post> Posts {  get; set; }
        public List<User> Members { get; set; }
        public bool IsClosed { get; set; }
    }
}
