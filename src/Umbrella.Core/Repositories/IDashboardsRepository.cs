using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;

namespace Umbrella.Core.Repositories;

public interface IDashboardsRepository
{
    Task<IEnumerable<Dashboard>?> GetAllAsync();
    Task<Dashboard?> GetAsync(string id);

    Task AddAsync(Dashboard dashboard);

    Task UpdateAsync(Dashboard dashboard);

    Task DeleteAync(string id);
}
