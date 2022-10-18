using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;

namespace Umbrella.Core.Services;

public interface IAreasService
{
    Task AddAsync(Area area);
    Task<IEnumerable<Area>> GetAllAsync();
}
