namespace SocialNetwork.Controllers
{
    [Route("api/group")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetGroupDto>>> Create(AddGroupDto newGroup)
        {
            return Ok(await _groupService.Create(newGroup));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<string>>> Delete([FromRoute] int id)
        {
            return Ok(await _groupService.Delete(id));
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ServiceResponse<GetGroupDto>>> Update([FromRoute] int id, [FromBody] UpdateGroupDto updatedGroup)
        {
            return Ok(await _groupService.Update(id, updatedGroup));
        }

        [HttpPost("{groupId}/kick/{memberId}")]
        public async Task<ActionResult<ServiceResponse<string>>> KickMember([FromRoute] int groupId, [FromRoute] int memberId)
        {
            return Ok(await _groupService.KickMember(groupId, memberId));
        }

        [HttpDelete("{groupId}/reject/{memberId}")]
        public async Task<ActionResult<ServiceResponse<string>>> RejectJoinRequest([FromRoute] int groupId, [FromRoute] int memberId)
        {
            return Ok(await _groupService.RejectJoinRequest(groupId, memberId));
        }

        [HttpPost("{groupId}/accept/{memberId}")]
        public async Task<ActionResult<ServiceResponse<string>>> AcceptJoinRequest([FromRoute] int groupId, [FromRoute] int memberId)
        {
            return Ok(await _groupService.AcceptJoinRequest(groupId, memberId));
        }

        [HttpPost("{groupId}")]
        public async Task<ActionResult<ServiceResponse<string>>> JoinGroup([FromRoute] int groupId)
        {
            return Ok(await _groupService.JoinGroup(groupId));
        }
        [HttpGet("{groupId}")]
        public async Task<ActionResult<ServiceResponse<GetGroupDto>>> GetOne([FromRoute] int groupId)
        {
            return Ok(await _groupService.GetOne(groupId));
        }
    }
}