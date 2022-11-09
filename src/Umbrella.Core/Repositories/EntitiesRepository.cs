using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Umbrella.Core.Models;

[assembly: InternalsVisibleTo("Umbrella.Core.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Umbrella.Core.Repositories;

public sealed class EntitiesRepository : IEntitiesRepository
{
    private const string RepositoryFilename = "entities.json";
    
    private readonly string _repositoryFullFilename;

    private ConcurrentDictionary<string, IEntity>? _entities = null;


    public EntitiesRepository(IConfigurationRepository configurationRepository)
    {
        _repositoryFullFilename = Path.Combine(configurationRepository.RepositoriesDirectory, RepositoryFilename);
    }

    
    public async Task<List<IEntity>?> GetAllAsync()
    {
        await LoadAsync();

        return _entities?.Values.ToList();
    }

    public async Task<IEntity?> GetAsync(string id)
    {
        await LoadAsync();

        return _entities is not null && _entities.ContainsKey(id) ? _entities[id] : null ;
    }

    public async Task<List<IEntity>?> GetAsync(EntityType type)
    {
        await LoadAsync();

        return _entities?.Values.Where(e => e.Type == type).ToList();
    }

    public async Task<int> GetCountAsync(string owner)
    {
        await LoadAsync();

        return _entities?.Count(e => e.Value.Owner == owner) ?? 0;
    }

    
    public async Task AddAsync(IEntity entity)
    {
        await LoadAsync();

        _entities!.TryAdd(entity.Id, entity);
        
        await SaveAsync();
    }

    public async Task AddAsync(List<IEntity> entities)
    {
        await LoadAsync();

        foreach (var entity in entities)
        {
            _entities!.TryAdd(entity.Id, entity);
        }

        await SaveAsync();
    }
    
    public async Task DeleteAsync(string id)
    {
        await LoadAsync();

        if (_entities!.ContainsKey(id))
        {
            _entities!.TryRemove(id, out _);
            await SaveAsync();
        }
    }

    public async Task DeleteByOwnerAsync(string owner)
    {
        await LoadAsync();

        foreach (var id in _entities!.Where(e => e.Value.Owner == owner).Select(e => e.Key))
        {
            _entities!.TryRemove(id, out _);
        }

        await SaveAsync();
    }


    private async Task LoadAsync()
    {
        if (_entities is not null)
        {
            return;
        }

        _entities = new ConcurrentDictionary<string, IEntity>();

        if (!File.Exists(_repositoryFullFilename))
        {
            return;
        }

        var json = await File.ReadAllTextAsync(_repositoryFullFilename);

        var offset = 0;
        while (true) { 
            var startIndex = json.IndexOf('{', offset);
            if (startIndex < 0)
            {
                break;
            }
            var endIndex = json.IndexOf('}', startIndex) + 1;
            var entityJson = json.Substring(startIndex, endIndex - startIndex);

            var baseEntity = JsonSerializer.Deserialize<EntityBase>(entityJson);
            switch (baseEntity?.Type)
            {
                case EntityType.Light:
                    var lightEntity = JsonSerializer.Deserialize<LightEntity>(entityJson);
                    if (lightEntity != null)
                    {
                        _entities.TryAdd(lightEntity.Id, lightEntity);
                    }
                    break;
                case EntityType.Weather:
                    var weatherEntity = JsonSerializer.Deserialize<WeatherEntity>(entityJson);
                    if (weatherEntity != null)
                    {
                        _entities.TryAdd(weatherEntity.Id, weatherEntity);
                    }
                    break;
                default:
                    break;
            }
            
            offset = endIndex + 1;
        }
    }

    private async Task SaveAsync()
    {
        var directoryName = Path.GetDirectoryName(_repositoryFullFilename);
        if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName)) {
            Directory.CreateDirectory(directoryName);
        }
        
        var json = new StringBuilder("[");
        for (int i = 0; i < _entities?.Count; i++)
        {
            if (i > 0)
            {
                json.Append(',');
            }
            var entity = _entities.ElementAt(i).Value;
            switch (entity.Type)
            {
                case EntityType.Light:
                    json.Append(JsonSerializer.Serialize(entity as LightEntity));
                    break;
                case EntityType.Weather:
                    json.Append(JsonSerializer.Serialize(entity as WeatherEntity));
                    break;
                default:
                    break;
            }
        }
        json.Append(']');
        await File.WriteAllTextAsync(_repositoryFullFilename, json.ToString());
    }
}
