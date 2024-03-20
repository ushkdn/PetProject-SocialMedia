namespace SocialNetwork.Dtos.GroupDtos
{
    public class GetGroupDto
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<GetMemberDto> Followers { get; set; }
        public bool IsClosed { get; set; }
    }
}