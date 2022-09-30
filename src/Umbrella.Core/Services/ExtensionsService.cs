using Umbrella.Core.Extensions;
using Umbrella.Core.Models;
using Umbrella.Core.Repositories;

namespace Umbrella.Core.Services;

public class ExtensionsService : IExtensionsService
{
    private readonly IExtensionRepository _extensionRepository;

    public ExtensionsService(IExtensionRepository extensionRepository)
    {
        _extensionRepository = extensionRepository;
    }

    public async Task<List<Extension>> GetAllRegisteredAsync()
    {
        return await _extensionRepository.GetAllAsync() ?? new List<Extension>();
    }

    public async Task RegisterAsync(IExtension extension, Dictionary<string, string?>? parameters)
    {
        await extension.RegisterAsync(parameters);
        await _extensionRepository.AddAsync(new Extension(extension.Id, parameters));
    }
}