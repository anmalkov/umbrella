namespace Umbrella.Core.Repositories;

public class ConfigurationRepository : IConfigurationRepository
{
    public string RepositoriesDirectory { get; private set; }

    public ConfigurationRepository()
    {
        RepositoriesDirectory = "./data";
    }
}
