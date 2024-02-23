namespace SocialNetwork.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Location { get; set; }
        public List<User> SentFriendRequests { get; set; }
        public List<User> IncomingFriendRequests {  get; set; }
        public List<Post> Posts { get; set; }
        public List<User> Friends { get; set; }
        public List<Group> Groups { get; set; }
    }
}
