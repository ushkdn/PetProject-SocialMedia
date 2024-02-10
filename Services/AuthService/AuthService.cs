
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.ComponentModel;
using System.Security.Cryptography;

namespace SocialNetwork.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public AuthService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context= context;
        }
        public async Task<ServiceResponse<GetUserDto>> Register(RegisterUserDto newUser) 
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try {
                var user = new User();
                var EmailCheck= await _context.Users.Where(x => x.Email == newUser.Email).FirstOrDefaultAsync();
                if (EmailCheck != null) {
                    throw new Exception("This email is already taken.");
                }
                user = _mapper.Map<User>(newUser);
                CreatePasswordHash(newUser.Password, out byte[] passwordSalt, out byte[] passwordHash);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetUserDto>(user);
                serviceResponse.Success = true;
                serviceResponse.Message = "You have successfully registered.";

            } catch(Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;

        }
        private static void CreatePasswordHash(string password, out byte[] passwordSalt, out byte[] passwordHash)
        {
            using (var hmac = new HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            
        }
        public async Task<ServiceResponse<string>> LoginIn (LoginInUserDto IncomingUser) {
            var serviceResponse = new ServiceResponse<string>();
            try {
                var user = new User();
                user = await _context.Users.Where(x => x.Email == IncomingUser.Email).FirstOrDefaultAsync() ?? throw new Exception("Wrong email or password.");
                if (!VerifyPassword(IncomingUser.Password, user.PasswordSalt, user.PasswordHash)) {
                    throw new Exception("Wrong email or password.");
                }
                serviceResponse.Data = "dsaff";
                serviceResponse.Success = true;
                serviceResponse.Message = "You are successfully logged in.";

            } catch (Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;

        }
        private static bool VerifyPassword(string password, byte[] passwordSalt, byte[] passwordHash) {
            using var hmac = new HMACSHA512(passwordSalt);
            var hashToCompute=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return passwordHash.SequenceEqual(hashToCompute);
        }

    }
}
