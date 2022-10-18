using System.Collections.Concurrent;
using System.Text.Json;
using System.Text;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;

namespace Umbrella.Core.Repositories;

public class AreasRepository : RepositoryBase<Area>, IAreasRepository
{
    private const string RepositoryFilename = "areas.json";

    public AreasRepository(IConfigurationRepository configurationRepository)
        : base(RepositoryFilename, configurationRepository) { }
}
