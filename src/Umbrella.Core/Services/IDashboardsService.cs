using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;

namespace Umbrella.Core.Services;

public interface IDashboardsService
{
    Task AddAsync(Dashboard dashboard);
    Task UpdateAsync(Dashboard dashboard);
    Task<IEnumerable<Dashboard>> GetAllAsync();
}
