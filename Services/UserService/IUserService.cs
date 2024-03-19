namespace SocialNetwork.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResponse<GetUserDto>> GetOne(int id);
    }
}