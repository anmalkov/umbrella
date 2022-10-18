using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;

namespace Umbrella.Core.Services;

public interface IGroupsService
{
    Task AddAsync(Group group);
    Task<IEnumerable<Group>> GetAllAsync();
}
