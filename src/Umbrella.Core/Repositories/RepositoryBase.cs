using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Umbrella.Core.Models;

namespace Umbrella.Core.Repositories;

public abstract class RepositoryBase<T> where T: class, IStorableItem
{
    private readonly string _repositoryFullFilename;

    private ConcurrentDictionary<string, T>? _items = null;


    public RepositoryBase(string repositoryFileName, IConfigurationRepository configurationRepository)
    {
        _repositoryFullFilename = Path.Combine(configurationRepository.RepositoriesDirectory, repositoryFileName);
    }

    public async Task<IEnumerable<T>?> GetAllAsync()
    {
        await LoadAsync();
        return _items?.Values;
    }

    public async Task<T?> GetAsync(string id)
    {
        await LoadAsync();
        return _items is not null && _items.ContainsKey(id) ? _items[id] : null;
    }
    
    public async Task AddAsync(T item)
    {
        await LoadAsync();

        _items!.TryAdd(item.Id, item);

        await SaveAsync();
    }

    public async Task UpdateAsync(T item)
    {
        await LoadAsync();

        if (_items!.ContainsKey(item.Id))
        {
            _items.TryRemove(item.Id, out _);
            _items.TryAdd(item.Id, item);
        }

        await SaveAsync();
    }

    public async Task DeleteAync(string id)
    {
        await LoadAsync();

        if (_items!.ContainsKey(id))
        {
            _items.TryRemove(id, out _);
            await SaveAsync();
        }
    }


    private async Task LoadAsync()
    {
        if (_items is not null)
        {
            return;
        }

        if (!File.Exists(_repositoryFullFilename))
        {
            _items = new();
            return;
        }

        var json = await File.ReadAllTextAsync(_repositoryFullFilename);
        _items = JsonSerializer.Deserialize<ConcurrentDictionary<string, T>>(json) ?? new();
    }
    
    private async Task SaveAsync()
    {
        var directoryName = Path.GetDirectoryName(_repositoryFullFilename);
        if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        var json = JsonSerializer.Serialize(_items);
        await File.WriteAllTextAsync(_repositoryFullFilename, json.ToString());
    }
}
