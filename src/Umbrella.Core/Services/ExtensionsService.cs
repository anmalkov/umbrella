using Umbrella.Core.Extensions;
using Umbrella.Core.Models;
using Umbrella.Core.Repositories;

namespace Umbrella.Core.Services;

public class ExtensionsService : IExtensionsService
{
    private readonly IExtensionRepository _extensionRepository;
    private readonly IEntitiesService _entitiesService;
    private readonly IEnumerable<IExtension> _extensions;

    public ExtensionsService(IExtensionRepository extensionRepository, IEntitiesService entitiesService, IEnumerable<IExtension> extensions)
    {
        _extensionRepository = extensionRepository;
        _entitiesService = entitiesService;
        _extensions = extensions;
    }

    public Task<IEnumerable<Extension>> GetAllAsync()
    {
        return Task.FromResult(_extensions.Select(e => new Extension(e.Id)
        {
            DisplayName = e.DisplayName,
            HtmlForRegistration = e.HtmlForRegistration,
            Image = e.Image
        }));
    }

    public async Task<IEnumerable<RegisteredExtension>> GetRegisteredAsync()
    {
        return await _extensionRepository.GetAllAsync() ?? new List<RegisteredExtension>();
    }    

    public async Task RegisterAsync(string id, IEnumerable<KeyValuePair<string, string?>>? parameters)
    {
        var extension = _extensions.FirstOrDefault(e => e.Id == id);
        if (extension is null)
        {
            return;
        }
        
        var paramsDictionary = parameters?.ToDictionary(p => p.Key, p => p.Value);
        
        await extension.RegisterAsync(paramsDictionary);
        await _extensionRepository.AddAsync(new RegisteredExtension(extension.Id, paramsDictionary));
        await extension.StartAsync(paramsDictionary);
    }

    public async Task UnregisterAsync(string id)
    {
        var extension = _extensions.FirstOrDefault(e => e.Id == id);
        if (extension is null)
        {
            return;
        }

        var registeredExtension = await _extensionRepository.GetAsync(extension.Id);
        if (registeredExtension is null)
        {
            return;
        }

        await extension.StopAsync();
        await extension.UnregisterAsync(registeredExtension.Parameters);
        await _entitiesService.DeleteByOwnerAsync(registeredExtension.Id);
        await _extensionRepository.DeleteAync(registeredExtension.Id);
    }
}