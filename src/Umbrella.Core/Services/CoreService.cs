using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Xml.Linq;
using Umbrella.Core.Models;
using Umbrella.Core.Repositories;

namespace Umbrella.Core.Services;

public sealed class CoreService : ICoreService
{
    private readonly IEntitiesService _entitiesService;

    public CoreService(IEntitiesService entitiesService)
    {
        _entitiesService = entitiesService;
    }

    public async Task RegisterEntityAsync(IEntity entity)
    {
        await _entitiesService.RegisterAsync(entity);
    }
    
    
}

