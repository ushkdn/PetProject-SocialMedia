namespace SocialNetwork.Services.GroupService
{
	public class GroupService : IGroupService
	{
		private readonly IMapper _mapper;
		private readonly DataContext _context;
		private readonly IHttpContextAccessor _http;

		public GroupService(IMapper mapper, DataContext context, IHttpContextAccessor http)
		{
			_mapper = mapper;
			_context = context;
			_http = http;
		}

		public async Task<ServiceResponse<GetGroupDto>> Create(AddGroupDto newGroup)
		{
			var serviceResponse = new ServiceResponse<GetGroupDto>();
			try
			{
				var group = _mapper.Map<Group>(newGroup);
				string ownerId = _http.HttpContext.User.FindFirstValue("Id") ?? throw new Exception("User not found.");
				group.OwnerId = ownerId;
				var groupOwner = await _context.Users.FindAsync(ownerId) ?? throw new Exception("User not found.");
				await _context.Groups.AddAsync(group);
				var userGroup = new UserGroups();
				groupOwner.Groups.Add(group);
				await _context.SaveChangesAsync();
				serviceResponse.Success = true;
				serviceResponse.Message = "You created a group.";
			}
			catch (Exception ex)
			{
				serviceResponse.Success = false;
				serviceResponse.Message = ex.Message;
			}
			return serviceResponse;
		}

		public async Task<ServiceResponse<string>> Delete(int id)
		{
			var serviceResponse = new ServiceResponse<string>();
			try
			{
				var group = await _context.Groups.FindAsync(id) ?? throw new Exception("Group not found.");

				if (group.OwnerId != _http.HttpContext.User.FindFirstValue("Id"))
				{
					throw new Exception("You are not group owner.");
				}

				_context.Groups.Remove(group);

				await _context.SaveChangesAsync();
				serviceResponse.Success = true;
				serviceResponse.Message = "Group deleted.";
			}
			catch (Exception ex)
			{
				serviceResponse.Success = false;
				serviceResponse.Message = ex.Message;
			}
			return serviceResponse;
		}

		public async Task<ServiceResponse<GetGroupDto>> Update(int id, UpdateGroupDto updatedGroup)
		{
			var serviceResponse = new ServiceResponse<GetGroupDto>();
			try
			{
				var groupToUpdate = await _context.Groups.FindAsync(id) ?? throw new Exception("Group not found.");
				if (groupToUpdate.OwnerId != _http.HttpContext.User.FindFirstValue("Id"))
				{
					throw new Exception("You are not group owner.");
				}
				foreach (var property in typeof(UpdateGroupDto).GetProperties())
				{
					var updatedValue = property.GetValue(updatedGroup);
					if (updatedValue != null && !string.IsNullOrEmpty(updatedValue.ToString()))
					{
						var groupProperty = groupToUpdate.GetType().GetProperty(property.Name);
						groupProperty.SetValue(groupToUpdate, updatedValue);
					}
				}
				_context.Groups.Update(groupToUpdate);
				await _context.SaveChangesAsync();
				serviceResponse.Data = _mapper.Map<GetGroupDto>(groupToUpdate);
				serviceResponse.Success = true;
				serviceResponse.Message = "Group successfully updated.";
			}
			catch (Exception ex)
			{
				serviceResponse.Success = false;
				serviceResponse.Message = ex.Message;
			}
			return serviceResponse;
		}

		public async Task<ServiceResponse<string>> JoinGroup(int groupId)
		{
			var serviceResponse = new ServiceResponse<string>();
			try
			{
				var group = await _context.Groups.FindAsync(groupId) ?? throw new Exception("Group not found.");
				var userId = _http.HttpContext.User.FindFirstValue("Id") ?? throw new Exception("User not found.");
				var user = await _context.Users.FindAsync(userId) ?? throw new Exception("User not found.");
				if (user.Id == group.OwnerId)
				{
					throw new Exception("You are group owner.");
				}
				if (user.Groups.Contains(group) || user.SentGroupJoinRequests.Contains(group))
				{
					throw new Exception("You are already in a group.");
				}
				if (group.IsClosed == false)
				{
					group.Followers.Add(user);
					user.Groups.Add(group);
					serviceResponse.Message = "You joined the group.";
				}
				else
				{
					group.JoinRequests.Add(user);
					user.SentGroupJoinRequests.Add(group);
					serviceResponse.Message = "Application to join the group has been sent.";
				}
				await _context.SaveChangesAsync();
				serviceResponse.Success = true;
			}
			catch (Exception ex)
			{
				serviceResponse.Success = false;
				serviceResponse.Message = ex.Message;
			}
			return serviceResponse;
		}

		public async Task<ServiceResponse<string>> AcceptJoinRequest(int groupId, string memberId)
		{
			var serviceResponse = new ServiceResponse<string>();
			try
			{
				var group = await _context.Groups.FindAsync(groupId) ?? throw new Exception("Group not found.");

				if (group.OwnerId != _http.HttpContext.User.FindFirstValue("Id"))
				{
					throw new Exception("You are not group owner.");
				}
				var userRequest = await _context.Users.FindAsync(memberId) ?? throw new Exception("User not found.");
				if (group.Followers.Contains(userRequest))
				{
					throw new Exception("User already in a group.");
				}
				group.Followers.Add(userRequest);
				userRequest.Groups.Add(group);
				userRequest.SentGroupJoinRequests.Remove(group);

				group.JoinRequests.Remove(userRequest);
				await _context.SaveChangesAsync();
				serviceResponse.Message = "You have approved the application.";
				serviceResponse.Success = true;
			}
			catch (Exception ex)
			{
				serviceResponse.Success = false;
				serviceResponse.Message = ex.Message;
			}
			return serviceResponse;
		}

		public async Task<ServiceResponse<string>> RejectJoinRequest(int groupId, string memberId)
		{
			var serviceResponse = new ServiceResponse<string>();
			try
			{
				var group = await _context.Groups.FindAsync(groupId) ?? throw new Exception("Group not found.");
				if (group.OwnerId != _http.HttpContext.User.FindFirstValue("Id"))
				{
					throw new Exception("You are not group owner.");
				}
				var userRequest = await _context.Users.FindAsync(memberId) ?? throw new Exception("User not found.");
				group.JoinRequests.Remove(userRequest);
				userRequest.SentGroupJoinRequests.Remove(group);
				await _context.SaveChangesAsync();
				serviceResponse.Message = "You rejected the application.";
				serviceResponse.Success = true;
			}
			catch (Exception ex)
			{
				serviceResponse.Success = false;
				serviceResponse.Message = ex.Message;
			}
			return serviceResponse;
		}

		public async Task<ServiceResponse<string>> KickMember(int groupId, string memberId)
		{
			var serviceResponse = new ServiceResponse<string>();
			try
			{
				var group = await _context.Groups.FindAsync(groupId) ?? throw new Exception("Group not found.");
				if (group.OwnerId != _http.HttpContext.User.FindFirstValue("Id"))
				{
					throw new Exception("You are not group owner.");
				}
				var user = await _context.Users.FindAsync(memberId) ?? throw new Exception("User not found.");
				if (!group.Followers.Contains(user))
				{
					throw new Exception("This member is not in the group.");
				}
				group.Followers.Remove(user);
				user.Groups.Remove(group);
				await _context.SaveChangesAsync();
				serviceResponse.Success = true;
				serviceResponse.Message = "Member successfully kicked out.";
			}
			catch (Exception ex)
			{
				serviceResponse.Success = false;
				serviceResponse.Message = ex.Message;
			}
			return serviceResponse;
		}

		public async Task<ServiceResponse<GetGroupDto>> GetOne(int id)
		{
			var serviceResponse = new ServiceResponse<GetGroupDto>();
			try
			{
				var group = await _context.Groups.FindAsync(id) ?? throw new Exception("Group not found.");
				serviceResponse.Data = _mapper.Map<GetGroupDto>(group);
				serviceResponse.Success = true;
				serviceResponse.Message = "You successfully recieved information about group.";
			}
			catch (Exception ex)
			{
				serviceResponse.Success = false;
				serviceResponse.Message = ex.Message;
			}
			return serviceResponse;
		}

		public async Task<ServiceResponse<List<GetGroupDto>>> GetAll()
		{
			var serviceResponse = new ServiceResponse<List<GetGroupDto>>();
			try
			{
				var groups = await _context.Groups.ToListAsync();
				serviceResponse.Data = groups.Select(x => _mapper.Map<GetGroupDto>(x)).ToList();
				serviceResponse.Success = true;
				serviceResponse.Message = "You successfully recieved information about group.";
			}
			catch (Exception ex)
			{
				serviceResponse.Success = false;
				serviceResponse.Message = ex.Message;
			}
			return serviceResponse;
		}
	}
}