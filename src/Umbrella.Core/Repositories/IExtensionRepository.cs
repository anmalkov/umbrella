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
    Task<List<Extension>?> GetAllAsync();
    Task<Extension?> GetAsync(string id);

    Task AddAsync(Extension extension);
    
    Task DeleteAync(string id);
}
