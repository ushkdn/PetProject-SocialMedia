using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetUserDto>>> GetOne([FromRoute]int id)
        {
            return Ok(await _userService.GetOne(id));
        }
        [HttpGet("{id}/friends")]
        public async Task<ActionResult<ServiceResponse<List<GetUserDto>>>> GetFriends([FromRoute] int id)
        {
            return Ok(await _userService.GetFriends(id));
        }
        [HttpPost("{id}/add-friend/{friendId}")]
        public async Task<ActionResult<ServiceResponse<string>>> SendFriendRequest([FromRoute] int id, [FromRoute] int friendId)
        {
            return Ok(await _userService.SendFriendRequest(id, friendId));
        }
        [HttpPost("{id}/friends/acceptRequest{requestUserId}")]
        public async Task<ActionResult<ServiceResponse<string>>> AcceptFriendRequest([FromRoute] int id, [FromRoute] int requestUserId)
        {
            return Ok(await _userService.AcceptFriendRequest(id, requestUserId));
        }
        [HttpDelete("{id}/friends/declinerequest{requestUserId}")]
        public async Task<ActionResult<ServiceResponse<string>>> DeclineFriendRequest([FromRoute] int id, [FromRoute] int requestUserId)
        {
            return Ok(await _userService.DeclineFriendRequest(id, requestUserId));
        }
        [HttpDelete("{id}/friends/delete{friendId}")]
        public async Task<ActionResult<ServiceResponse<string>>> DeleteFriend([FromRoute] int id, [FromRoute] int friendId)
        {
            return Ok(await _userService.DeleteFriend(id, friendId));
        }
    }
}
