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
    public Task<List<Extension>?> GetAllAsync();

    public Task AddAsync(Extension extension);
}
