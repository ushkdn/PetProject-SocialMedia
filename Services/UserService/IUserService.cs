namespace SocialNetwork.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResponse<string>> SendFriendRequest(int userId, int friendId);
        Task<ServiceResponse<string>> AcceptFriendRequest(int id, int requestUserId);
        Task<ServiceResponse<string>> DeclineFriendRequest(int id, int requestUserId);
        Task<ServiceResponse<string>> DeleteFriend(int id, int friendId);
        Task<ServiceResponse<List<GetUserDto>>> GetFriends(int id);
        Task<ServiceResponse<GetUserDto>> GetOne(int id);
    }
}
