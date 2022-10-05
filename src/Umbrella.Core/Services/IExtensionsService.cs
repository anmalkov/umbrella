using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;

namespace Umbrella.Core.Services;

public interface IExtensionsService
{
    Task RegisterAsync(IExtension extension, Dictionary<string, string?>? parameters);
    Task UnregisterAsync(IExtension extension);
    Task<IEnumerable<RegisteredExtension>> GetRegisteredAsync();
    Task<IEnumerable<IExtension>> GetAllAsync();
}
