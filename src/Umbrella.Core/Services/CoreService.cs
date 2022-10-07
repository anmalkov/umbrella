using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Extensions;

namespace Umbrella.Core.Services;

public class CoreService : ICoreService
{
    private readonly IExtensionsService _extensionsService;
    private readonly IEnumerable<IExtension> _extensions;
    private readonly IEntitiesStateService _entitiesStateService;

    public CoreService(IExtensionsService extensionsService, IEnumerable<IExtension> extensions, IEntitiesStateService entitiesStateService)
    {
        _extensionsService = extensionsService;
        _extensions = extensions;
        _entitiesStateService = entitiesStateService;
    }
    
    public async Task StartAsync()
    {
        _entitiesStateService.StartMonitoring();
        var registeredExtensions = await _extensionsService.GetRegisteredAsync();
        if (registeredExtensions is not null)
        {
            foreach (var registeredExtension in registeredExtensions)
            {
                var extension = _extensions.FirstOrDefault(e => e.Id == registeredExtension.Id);
                if (extension is not null)
                {
                    await extension.StartAsync(registeredExtension.Parameters);
                }
            }
        }
    }

    public async Task StopAsync()
    {
        var registeredExtensions = await _extensionsService.GetRegisteredAsync();
        if (registeredExtensions is not null)
        {
            foreach (var registeredExtension in registeredExtensions)
            {
                var extension = _extensions.FirstOrDefault(e => e.Id == registeredExtension.Id);
                if (extension is not null)
                {
                    await extension.StopAsync();
                }
            }
        }
        _entitiesStateService.StopMonitoring();
    }
}
