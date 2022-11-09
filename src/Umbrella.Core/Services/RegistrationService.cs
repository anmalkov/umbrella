using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Xml.Linq;
using Umbrella.Core.Models;
using Umbrella.Core.Repositories;

namespace Umbrella.Core.Services;

public sealed class RegistrationService : IRegistrationService
{
    private readonly IEntitiesService _entitiesService;
    private readonly IAreasService _areasService;
    private readonly IGroupsService _groupsService;

    public RegistrationService(IEntitiesService entitiesService, IAreasService areasService, IGroupsService groupsService)
    {
        _entitiesService = entitiesService;
        _areasService = areasService;
        _groupsService = groupsService;
    }

    public async Task RegisterEntityAsync(IEntity entity, string extensionId)
    {
        entity.Owner = extensionId;
        await _entitiesService.RegisterAsync(entity);
    }

    public async Task<IEntity?> GetEntityAsync(string id, string extensionId)
    {
        var entity = await _entitiesService.GetAsync(id);
        return entity is not null && entity.Owner == extensionId ? entity : null;
    }

    public async Task AddAreaAsync(Area area, string extensionId)
    {
        await _areasService.AddAsync(area);
    }

    public async Task AddGroupAsync(Group group, string extensionId)
    {
        await _groupsService.AddAsync(group);
    }
}
