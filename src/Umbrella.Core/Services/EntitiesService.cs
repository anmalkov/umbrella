using System.Runtime.CompilerServices;
using Umbrella.Core.Models;
using Umbrella.Core.Repositories;

[assembly: InternalsVisibleTo("Umbrella.Core.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Umbrella.Core.Services;

public sealed class EntitiesService : IEntitiesService
{
    private readonly IEntitiesRepository _entitiesRepository;
    private readonly IEntitiesStateService _stateService;

    public EntitiesService(IEntitiesRepository entitiesRepository, IEntitiesStateService stateService)
    {
        _entitiesRepository = entitiesRepository;
        _stateService = stateService;
    }

        
    public async Task<List<IEntity>?> GetAsync(EntityType type)
    {
        return await _entitiesRepository.GetAsync(type);
    }

    public async Task<List<IEntity>?> GetAllAsync()
    {
        return await _entitiesRepository.GetAllAsync();
    }

    public async Task<IEntity?> GetAsync(string id)
    {
        return await _entitiesRepository.GetAsync(id);
    }

    public async Task<int> GetCount(string owner)
    {
        return await _entitiesRepository.GetCountAsync(owner);
    }

    public IEntityState? GetState(string id)
    {
        return _stateService.GetState(id);
    }

    public IDictionary<string, IEntityState>? GetStates()
    {
        return _stateService.GetStates();
    }

    
    public async Task RegisterAsync(IEntity entity)
    {
        if (await _entitiesRepository.GetAsync(entity.Id) is not null)
        {
            throw new ArgumentException("", "Id");
        }
        await _entitiesRepository.AddAsync(entity);
    }

    public Task UpdateAsync(IEntity entity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(string id)
    {
        await _entitiesRepository.DeleteAsync(id);
    }

    public async Task DeleteByOwnerAsync(string owner)
    {
        await _entitiesRepository.DeleteByOwnerAsync(owner);
    }
}
