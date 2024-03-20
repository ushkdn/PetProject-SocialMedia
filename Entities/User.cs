namespace SocialNetwork.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Surname { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<Group> Groups { get; set; } = [];
        public List<Group> SentGroupJoinRequests { get; set; } = [];
    }
}