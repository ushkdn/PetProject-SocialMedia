namespace SocialNetwork.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<GetUserDto>> GetOne(string id)
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                var user = await _context.Users.FindAsync(id) ?? throw new Exception("User not found.");
                serviceResponse.Data = _mapper.Map<GetUserDto>(user);
                serviceResponse.Success = true;
                serviceResponse.Message = "You have successfully received information about yourself.";
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}