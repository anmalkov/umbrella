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

    public RegistrationService(IEntitiesService entitiesService)
    {
        _entitiesService = entitiesService;
    }

    public async Task RegisterEntityAsync(IEntity entity, string extensionId)
    {
        entity.Owner = extensionId;
        await _entitiesService.RegisterAsync(entity);
    }
}
