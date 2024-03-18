namespace SocialNetwork.Services.GroupService
{
    public interface IGroupService
    {
        Task<ServiceResponse<GetGroupDto>> Create(AddGroupDto newGroup);

        Task<ServiceResponse<GetGroupDto>> Update(int id, UpdateGroupDto updatedGroup);

        Task<ServiceResponse<string>> Delete(int id);

        Task<ServiceResponse<string>> KickMember(int groupId, string memberId);

        Task<ServiceResponse<string>> RejectJoinRequest(int groupId, string memberId);

        Task<ServiceResponse<string>> AcceptJoinRequest(int groupId, string memberId);

        Task<ServiceResponse<string>> JoinGroup(int groupId);
    }
}