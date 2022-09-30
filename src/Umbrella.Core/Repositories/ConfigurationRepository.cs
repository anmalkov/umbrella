using System.Reflection;

namespace Umbrella.Core.Repositories;

public class ConfigurationRepository : IConfigurationRepository
{
    public string RepositoriesDirectory { get; private set; }

    public ConfigurationRepository()
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
        RepositoriesDirectory = Path.Combine(currentDirectory, "data");
    }
}
