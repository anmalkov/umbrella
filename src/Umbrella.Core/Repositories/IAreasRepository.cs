using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;

namespace Umbrella.Core.Repositories;

public interface IAreasRepository
{
    Task<IEnumerable<Area>?> GetAllAsync();
    Task<Area?> GetAsync(string id);

    Task AddAsync(Area area);
    
    Task DeleteAync(string id);
}
