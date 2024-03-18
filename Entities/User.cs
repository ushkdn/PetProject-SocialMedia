namespace SocialNetwork.Entities
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Location { get; set; } = string.Empty;

        //public List<User> Friends { get; set; } = [];
        public List<UserGroups> Groups { get; set; } = [];

        public List<UserJoinRequest> SentGroupJoinRequests { get; set; } = [];
        //public List<int> SentFriendRequests { get; set; } = [];
        //public List<int> IncomingFriendRequests { get; set; } = [];
    }
}