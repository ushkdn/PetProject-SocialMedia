using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Entities
{
    public class UserJoinRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public User User { get; set; }
        public Group Group { get; set; }
    }
}