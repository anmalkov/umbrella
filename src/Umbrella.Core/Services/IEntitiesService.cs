using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Models;

namespace Umbrella.Core.Services;

public interface IEntitiesService
{
    Task RegisterAsync(IEntity entity);
    Task UpdateAsync(IEntity entity);
    Task DeleteAsync(string id);
    Task DeleteByOwnerAsync(string owner);
    Task<IEntity?> GetAsync(string id);
    Task<List<IEntity>?> GetAllAsync();
    Task<List<IEntity>?> GetAsync(EntityType type);
	Task<int> GetCount(string owner);
    IEntityState? GetState(string id);
}
