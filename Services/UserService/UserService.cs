namespace SocialNetwork.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        //todo: fix all methods
        public UserService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<GetUserDto>> GetOne(int id)
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try {
                var user = await _context.Users.FindAsync(id) ?? throw new Exception("User not found.");
                serviceResponse.Data=_mapper.Map<GetUserDto>(user);
                serviceResponse.Success = true;
                serviceResponse.Message = "You have successfully received information about yourself.";

            } catch (Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<List<GetUserDto>>> GetFriends(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetUserDto>>();
            try {
                var user = await _context.Users.FindAsync(id) ?? throw new Exception("User not found.");
                var friends = user.Friends;
                serviceResponse.Data = friends.Select(x => _mapper.Map<GetUserDto>(x)).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = "You have successfully received information about your friends.";
            }catch(Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success= false;
                serviceResponse.Message= ex.Message;
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<string>> SendFriendRequest(int userId, int friendId)
        {
            var serviceResponse = new ServiceResponse<string>();
            try {
                var user = await _context.Users.FindAsync(userId) ?? throw new Exception("User not found.");
                var friend = await _context.Users.FindAsync(friendId) ?? throw new Exception("User not found.");
                user.SentFriendRequests.Add(friend);
                friend.IncomingFriendRequests.Add(user);
                await _context.SaveChangesAsync();
                serviceResponse.Data = null;
                serviceResponse.Success = true;
                serviceResponse.Message = "Friend request sent.";
            } catch(Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success= false;
                serviceResponse.Message= ex.Message;
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<string>> AcceptFriendRequest(int id, int requestUserId)
        {
            var serviceResponse = new ServiceResponse<string>();
            try {
                var user = await _context.Users.FindAsync(id) ?? throw new Exception("User not found.");
                var friend = await _context.Users.FindAsync(requestUserId) ?? throw new Exception("User not found.");
                user.Friends.Add(friend);
                user.IncomingFriendRequests.Remove(friend);
                friend.SentFriendRequests.Remove(user);
                await _context.SaveChangesAsync();
                serviceResponse.Data = null;
                serviceResponse.Success = true;
                serviceResponse.Message = "Friend added.";
            } catch (Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<string>> DeclineFriendRequest(int id, int requestUserId)
        {
            var serviceResponse = new ServiceResponse<string>();
            try {
                var user = await _context.Users.FindAsync(id) ?? throw new Exception("User not found.");
                var friend = await _context.Users.FindAsync(requestUserId) ?? throw new Exception("User not found.");
                user.IncomingFriendRequests.Remove(friend);
                friend.SentFriendRequests.Remove(user);
                friend.SentFriendRequests.Remove(user);
                await _context.SaveChangesAsync();
                serviceResponse.Data = null;
                serviceResponse.Success = true;
                serviceResponse.Message = "Friend request rejected.";
            } catch (Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<string>> DeleteFriend(int id, int friendId)
        {
            var serviceResponse = new ServiceResponse<string>();
            try {
                var user = await _context.Users.FindAsync(id) ?? throw new Exception("User not found.");
                var friend = await _context.Users.FindAsync(friendId) ?? throw new Exception("User not found.");
                user.Friends.Remove(friend);
                friend.Friends.Remove(user);
                await _context.SaveChangesAsync();
                serviceResponse.Data = null;
                serviceResponse.Success = true;
                serviceResponse.Message = "Friend request rejected.";
            } catch (Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}
