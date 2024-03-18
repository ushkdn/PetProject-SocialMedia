namespace SocialNetwork.Dtos.GroupDtos
{
    public class AddGroupDto
    {
        [Required, MinLength(5)]
        public string Name { get; set; }

        [Required, MinLength(20), MaxLength(150)]
        public string Description { get; set; }

        [Required]
        public bool IsClosed { get; set; }
    }
}