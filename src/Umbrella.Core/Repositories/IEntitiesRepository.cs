using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Models;

namespace Umbrella.Core.Repositories;

public interface IEntitiesRepository
{
    Task<List<IEntity>?> GetAllAsync();
    Task<IEntity?> GetAsync(string id);
    Task<List<IEntity>?> GetAsync(EntityType type);
    Task<int> GetCountAsync(string owner);

    Task AddAsync(IEntity entity);
    Task AddAsync(List<IEntity> entities);
    
    Task DeleteAsync(string id);
    Task DeleteByOwnerAsync(string owner);
}
