using System.Collections.Concurrent;
using System.Text.Json;
using System.Text;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;

namespace Umbrella.Core.Repositories;

public class DashboardsRepository : RepositoryBase<Dashboard>, IDashboardsRepository
{
    private const string RepositoryFilename = "dashboards.json";

    public DashboardsRepository(IConfigurationRepository configurationRepository)
        : base(RepositoryFilename, configurationRepository) { }
}
