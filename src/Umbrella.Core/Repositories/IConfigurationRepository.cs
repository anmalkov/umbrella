using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Repositories;

public interface IConfigurationRepository
{
    public string RepositoriesDirectory { get; }
}
