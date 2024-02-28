using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<int> Posts { get; set; } = [];
        public List<int> Followers { get; set; } = [];
        public List<int> JoinRequests{ get; set; } = [];
        public bool IsClosed { get; set; }
    }
}
