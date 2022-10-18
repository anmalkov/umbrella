using System.Collections.Concurrent;
using System.Text.Json;
using System.Text;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;

namespace Umbrella.Core.Repositories;

public class GroupsRepository : RepositoryBase<Group>, IGroupsRepository
{
    private const string RepositoryFilename = "groups.json";

    public GroupsRepository(IConfigurationRepository configurationRepository)
        : base(RepositoryFilename, configurationRepository) { }
}
