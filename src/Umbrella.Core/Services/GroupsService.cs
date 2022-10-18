using Umbrella.Core.Extensions;
using Umbrella.Core.Models;
using Umbrella.Core.Repositories;

namespace Umbrella.Core.Services;

public class GroupsService : IGroupsService
{
    private readonly IGroupsRepository _groupsRepository;

    public GroupsService(IGroupsRepository groupsRepository)
    {
        _groupsRepository = groupsRepository;
    }

    public async Task<IEnumerable<Group>> GetAllAsync()
    {
        return await _groupsRepository.GetAllAsync() ?? new List<Group>();
    }    

    public async Task AddAsync(Group group)
    {
        await _groupsRepository.AddAsync(group);
    }
}