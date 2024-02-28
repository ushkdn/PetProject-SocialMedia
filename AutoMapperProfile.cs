namespace SocialNetwork
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AddGroupDto, Group>();
            CreateMap<Group, GetGroupDto>();
            CreateMap<RegisterUserDto, User>();
            CreateMap<User, GetUserDto>();
            CreateMap<UpdateUserDto, User>();
        }
    }
}
