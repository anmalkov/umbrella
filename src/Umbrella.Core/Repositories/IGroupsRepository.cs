using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;

namespace Umbrella.Core.Repositories;

public interface IGroupsRepository
{
    Task<IEnumerable<Group>?> GetAllAsync();
    Task<Group?> GetAsync(string id);

    Task AddAsync(Group group);
    
    Task DeleteAync(string id);
}
