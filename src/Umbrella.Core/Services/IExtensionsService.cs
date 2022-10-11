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
    Task RegisterAsync(string id, IEnumerable<KeyValuePair<string, string?>>? parameters);
    Task UnregisterAsync(string id);
    Task<IEnumerable<RegisteredExtension>> GetRegisteredAsync();
    Task<IEnumerable<Extension>> GetAllAsync();
}
