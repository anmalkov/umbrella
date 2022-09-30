using Umbrella.Core.Extensions;
using Umbrella.Core.Models;
using Umbrella.Core.Repositories;

namespace Umbrella.Core.Services;

public class ExtensionsService : IExtensionsService
{
    private readonly IExtensionRepository _extensionRepository;
    private readonly IEntitiesService _entitiesService;

    public ExtensionsService(IExtensionRepository extensionRepository, IEntitiesService entitiesService)
    {
        _extensionRepository = extensionRepository;
        _entitiesService = entitiesService;
    }

    public async Task<List<Extension>> GetRegisteredAsync()
    {
        return await _extensionRepository.GetAllAsync() ?? new List<Extension>();
    }

    public async Task RegisterAsync(IExtension extension, Dictionary<string, string?>? parameters)
    {
        await extension.RegisterAsync(parameters);
        await _extensionRepository.AddAsync(new Extension(extension.Id, parameters));
    }

    public async Task UnregisterAsync(IExtension extension)
    {
        var registeredExtension = await _extensionRepository.GetAsync(extension.Id);
        if (registeredExtension is null)
        {
            return;
        }

        await extension.UnregisterAsync(registeredExtension.Parameters);
        await _entitiesService.DeleteByOwnerAsync(registeredExtension.Id);
        await _extensionRepository.DeleteAync(registeredExtension.Id);
    }
}