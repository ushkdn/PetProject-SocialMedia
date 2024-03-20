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
			//working too
			try
			{
				string id = _http.HttpContext.User.FindFirstValue("Id") ?? throw new Exception("You are not log-in.");
				if (!int.TryParse(id, out int ownerId)) { throw new Exception("User not found."); }
				var group = _mapper.Map<Group>(newGroup);
				group.OwnerId = ownerId;
				var user = await _context.Users.FindAsync(ownerId) ?? throw new Exception("User not found.");
				user.Groups.Add(group);
				await _context.SaveChangesAsync();
				var groupDto = _mapper.Map<GetGroupDto>(group);
				groupDto.Followers = new List<GetMemberDto> { _mapper.Map<GetMemberDto>(user) };
				serviceResponse.Data = groupDto;
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
			//working
			var serviceResponse = new ServiceResponse<string>();
			try
			{
				string _userId = _http.HttpContext.User.FindFirstValue("Id") ?? throw new Exception("You are not log-in.");
				if (!int.TryParse(_userId, out int userId)) { throw new Exception("User not found."); }
				var group = await _context.Groups.FindAsync(id) ?? throw new Exception("Group not found.");
				if (group.OwnerId != userId)
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
				if (groupToUpdate.OwnerId != Convert.ToInt32(_http.HttpContext.User.FindFirstValue("Id")))
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
			//working
			var serviceResponse = new ServiceResponse<string>();
			try
			{
				string id = _http.HttpContext.User.FindFirstValue("Id") ?? throw new Exception("You are not log-in.");
				if (!int.TryParse(id, out int userId)) { throw new Exception("User not found."); }
				var group = await _context.Groups.FindAsync(groupId) ?? throw new Exception("Group not found.");
				var user = await _context.Users.Include(g => g.SentGroupJoinRequests).Include(g => g.Groups).FirstOrDefaultAsync(x => x.Id == userId) ?? throw new Exception("User not found.");
				if (user.Id == group.OwnerId)
				{
					throw new Exception("You are group owner.");
				}
				if (user.Groups.Contains(group))
				{
					throw new Exception("You are already in the group.");
				}
				if (user.SentGroupJoinRequests.Contains(group))
				{
					throw new Exception("Join request already sent.");
				}

				if (group.IsClosed == false)
				{
					user.Groups.Add(group);
					serviceResponse.Message = "You joined the group.";
				}

				if (group.IsClosed == true)
				{
					user.SentGroupJoinRequests.Add(group);
					serviceResponse.Message = "You sent join request.";

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

		public async Task<ServiceResponse<string>> AcceptJoinRequest(int groupId, int memberId)
		{
			//working
			var serviceResponse = new ServiceResponse<string>();
			try
			{
				string id = _http.HttpContext.User.FindFirstValue("Id") ?? throw new Exception("You are not log-in.");
				if (!int.TryParse(id, out int ownerId)) { throw new Exception("User not found."); }
				var group = await _context.Groups.FindAsync(groupId) ?? throw new Exception("Group not found.");
				if (group.OwnerId != ownerId)
				{
					throw new Exception("You are not group owner.");
				}
				var user = await _context.Users.Include(g => g.SentGroupJoinRequests).Include(g => g.Groups).FirstOrDefaultAsync(x => x.Id == memberId) ?? throw new Exception("User not found.");
				if (group.Followers.Contains(user))
				{
					throw new Exception("User already in a group.");
				}
				group.Followers.Add(user);
				user.SentGroupJoinRequests.Remove(group);
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

		public async Task<ServiceResponse<string>> RejectJoinRequest(int groupId, int memberId)
		{
			//working
			var serviceResponse = new ServiceResponse<string>();
			try
			{
				string id = _http.HttpContext.User.FindFirstValue("Id") ?? throw new Exception("You are not log-in.");
				if (!int.TryParse(id, out int userId)) { throw new Exception("User not found."); }
				var group = await _context.Groups.FindAsync(groupId) ?? throw new Exception("Group not found.");
				if (group.OwnerId != userId)
				{
					throw new Exception("You are not group owner.");
				}
				var user = await _context.Users.Include(r => r.SentGroupJoinRequests).FirstOrDefaultAsync(x => x.Id == memberId) ?? throw new Exception("User not found.");
				if (!user.SentGroupJoinRequests.Contains(group))
				{
					throw new Exception("Join request not found.");
				}
				user.SentGroupJoinRequests.Remove(group);
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

		public async Task<ServiceResponse<string>> KickMember(int groupId, int memberId)
		{
			//working
			var serviceResponse = new ServiceResponse<string>();
			try
			{
				string id = _http.HttpContext.User.FindFirstValue("Id") ?? throw new Exception("You are not log-in.");
				if (!int.TryParse(id, out int userId)) { throw new Exception("User not found."); }
				var group = await _context.Groups.FindAsync(groupId) ?? throw new Exception("Group not found.");

				if (group.OwnerId != userId)
				{
					throw new Exception("You are not group owner.");
				}
				if (memberId == group.OwnerId)
				{
					throw new Exception("You are group owner.");
				}
				var user = await _context.Users.Include(g => g.Groups).FirstOrDefaultAsync(x => x.Id == memberId) ?? throw new Exception("User not found.");
				if (!group.Followers.Contains(user))
				{
					throw new Exception("This member is not in the group.");
				}
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

		public async Task<ServiceResponse<GetGroupDto>> GetOne(int groupId)
		{
			//working
			var serviceResponse = new ServiceResponse<GetGroupDto>();
			try
			{
				var group = await _context.Groups.Include(f => f.Followers).FirstOrDefaultAsync(x => x.Id == groupId) ?? throw new Exception("Group not found.");
				var groupDto = _mapper.Map<GetGroupDto>(group);
				serviceResponse.Data = groupDto;
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
			//working
			var serviceResponse = new ServiceResponse<List<GetGroupDto>>();
			try
			{
				var groups = await _context.Groups.ToListAsync();
				serviceResponse.Data = groups.Select(x => _mapper.Map<GetGroupDto>(x)).ToList();
				serviceResponse.Success = true;
				serviceResponse.Message = "You recieved information about group.";
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