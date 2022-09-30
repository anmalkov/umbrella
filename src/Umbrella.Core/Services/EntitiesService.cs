using System.Runtime.CompilerServices;
using Umbrella.Core.Models;
using Umbrella.Core.Repositories;

[assembly: InternalsVisibleTo("Umbrella.Core.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Umbrella.Core.Services;

public sealed class EntitiesService : IEntitiesService
{
    private readonly IEntitiesRepository _entitiesRepository;
    

    public EntitiesService(IEntitiesRepository entitiesRepository)
    {
        _entitiesRepository = entitiesRepository;
    }
        

    public Task DeleteAsync(string id)
    {
        throw new NotImplementedException();
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

    public async Task<int> GetCount(string owner)
    {
        return await _entitiesRepository.GetCountAsync(owner);
    }
}
