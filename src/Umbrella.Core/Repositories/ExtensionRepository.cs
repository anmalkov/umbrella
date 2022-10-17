using System.Collections.Concurrent;
using System.Text.Json;
using System.Text;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;

namespace Umbrella.Core.Repositories;

public class ExtensionRepository : RepositoryBase<RegisteredExtension>, IExtensionRepository
{
    private const string RepositoryFilename = "extensions.json";

    public ExtensionRepository(IConfigurationRepository configurationRepository)
        : base(RepositoryFilename, configurationRepository) { }
}
