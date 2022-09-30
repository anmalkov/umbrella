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

    public async Task AddAsync(Extension extension)
    {
        await LoadAsync();

        if (_extensions is null)
        {
            _extensions = new ConcurrentDictionary<string, Extension>();
        }

        _extensions.TryAdd(extension.Id, extension);

        await SaveAsync();
    }


    private async Task LoadAsync()
    {
        if (_extensions is not null || !File.Exists(_repositoryFullFilename))
        {
            return;
        }

        var json = await File.ReadAllTextAsync(_repositoryFullFilename);
        _extensions = JsonSerializer.Deserialize<ConcurrentDictionary<string, Extension>>(json);
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