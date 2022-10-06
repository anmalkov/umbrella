using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Extensions;

namespace Umbrella.Core.Services;

public class CoreService : ICoreService, IDisposable
{
    private Timer? _timer = null;
    private readonly IExtensionsService _extensionsService;
    private readonly IEnumerable<IExtension> _extensions;

    public CoreService(IExtensionsService extensionsService, IEnumerable<IExtension> extensions)
    {
        _extensionsService = extensionsService;
        _extensions = extensions;
    }
    
    public async Task StartAsync()
    {
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
        
        //_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    public Task StopAsync()
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
    }
    

    private void DoWork(object? state)
    {
        
    }
}
