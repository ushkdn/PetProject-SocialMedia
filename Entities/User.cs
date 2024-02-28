namespace SocialNetwork.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Surname { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<int> Friends{ get; set; } = [];
        public List<int> Groups { get; set; } = [];
        public List<int> SentGroupJoinRequests {  get; set; } = [];
        public List<int> SentFriendRequests { get; set; } = [];
        public List<int> IncomingFriendRequests { get; set; } = [];
    }
}
