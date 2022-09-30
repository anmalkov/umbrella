using System.Collections.Concurrent;
using System.Text.Json;
using System.Text;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;

namespace Umbrella.Core.Repositories;

public class ExtensionRepository : IExtensionRepository
{
    private const string RepositoryFilename = "extensions.json";

    private readonly string _repositoryFullFilename;

    private ConcurrentDictionary<string, Extension>? _extensions = null;


    public ExtensionRepository(IConfigurationRepository configurationRepository)
    {
        _repositoryFullFilename = Path.Combine(configurationRepository.RepositoriesDirectory, RepositoryFilename);
    }

    public async Task<List<Extension>?> GetAllAsync()
    {
        await LoadAsync();
        return _extensions?.Values.ToList();
    }

    public async Task<Extension?> GetAsync(string id)
    {
        await LoadAsync();
        return _extensions is not null && _extensions.ContainsKey(id) ? _extensions[id] : null;
    }

    public async Task AddAsync(Extension extension)
    {
        await LoadAsync();

        _extensions!.TryAdd(extension.Id, extension);

        await SaveAsync();
    }

    public async Task DeleteAync(string id)
    {
        await LoadAsync();

        if (_extensions!.ContainsKey(id))
        {
            _extensions.TryRemove(id, out _);
            await SaveAsync();
        }
    }


    private async Task LoadAsync()
    {
        if (_extensions is not null)
        {
            return;
        }

        if (!File.Exists(_repositoryFullFilename))
        {
            _extensions = new ConcurrentDictionary<string, Extension>();
            return;
        }

        var json = await File.ReadAllTextAsync(_repositoryFullFilename);
        _extensions = JsonSerializer.Deserialize<ConcurrentDictionary<string, Extension>>(json);

        if (_extensions is null)
        {
            _extensions = new ConcurrentDictionary<string, Extension>();
        }
    }

    private async Task SaveAsync()
    {
        var directoryName = Path.GetDirectoryName(_repositoryFullFilename);
        if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        var json = JsonSerializer.Serialize(_extensions);
        await File.WriteAllTextAsync(_repositoryFullFilename, json.ToString());
    }
}
