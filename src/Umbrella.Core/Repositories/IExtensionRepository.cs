using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;

namespace Umbrella.Core.Repositories;

public interface IExtensionRepository
{
    Task<IEnumerable<RegisteredExtension>?> GetAllAsync();
    Task<RegisteredExtension?> GetAsync(string id);

    Task AddAsync(RegisteredExtension extension);
    
    Task DeleteAync(string id);
}
