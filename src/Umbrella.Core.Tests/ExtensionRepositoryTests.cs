using Umbrella.Core.Models;
using Umbrella.Core.Repositories;

namespace Umbrella.Core.Tests
{
    public class ExtensionsRepositoryTests
    {
        private const string RepositoryFilename = "extensions.json";

        [Fact]
        public async Task Add_creates_file()
        {
            var path = "./unit-tests";
            var fullFilename = Path.Combine(path, RepositoryFilename);

            var extension = new RegisteredExtension("ex1");

            if (File.Exists(fullFilename))
            {
                File.Delete(fullFilename);
            }

            var config = new Mock<IConfigurationRepository>();
            config.SetupGet(x => x.RepositoriesDirectory).Returns(path);

            var repository = new ExtensionRepository(config.Object);
            await repository.AddAsync(extension);

            Assert.True(File.Exists(fullFilename));
        }

        [Fact]
        public async Task GetAll_returns_all_extensions()
        {
            var path = "./unit-tests";
            var fullFilename = Path.Combine(path, RepositoryFilename);

            var extensions = new List<RegisteredExtension>
            {
                new RegisteredExtension("ex1", new Dictionary<string, string?> { { "param1", "value1" }, { "param2", "value2" } }),
                new RegisteredExtension("ex2", new Dictionary<string, string?> { { "param3", "value3" }, { "param4", "value4" } }),
                new RegisteredExtension("ex3")
            };

            if (File.Exists(fullFilename))
            {
                File.Delete(fullFilename);
            }

            var config = new Mock<IConfigurationRepository>();
            config.SetupGet(x => x.RepositoriesDirectory).Returns(path);

            var repository = new ExtensionRepository(config.Object);
            foreach (var extension in extensions)
            {
                await repository.AddAsync(extension);
            }

            var loadedExtensions = await repository.GetAllAsync();

            Assert.Equal(extensions.Count, loadedExtensions?.Count);

            foreach (var extension in extensions)
            {
                var loadedExtension = loadedExtensions?.FirstOrDefault(e => e.Id == extension.Id);
                Assert.Equivalent(extension, loadedExtension);
            }
        }

        [Fact]
        public async Task Delete_removes_the_extension()
        {
            var path = "./unit-tests";
            var fullFilename = Path.Combine(path, RepositoryFilename);

            var extensions = new List<RegisteredExtension>
            {
                new RegisteredExtension("ex1", new Dictionary<string, string?> { { "param1", "value1" }, { "param2", "value2" } }),
                new RegisteredExtension("ex2", new Dictionary<string, string?> { { "param3", "value3" }, { "param4", "value4" } }),
            };

            if (File.Exists(fullFilename))
            {
                File.Delete(fullFilename);
            }

            var config = new Mock<IConfigurationRepository>();
            config.SetupGet(x => x.RepositoriesDirectory).Returns(path);

            var repository = new ExtensionRepository(config.Object);
            foreach (var extension in extensions)
            {
                await repository.AddAsync(extension);
            }

            await repository.DeleteAync("ex1");

            var loadedExtensions = await repository.GetAllAsync();

            Assert.NotNull(loadedExtensions);
            Assert.Single(loadedExtensions!);
            Assert.Equivalent(extensions[1], loadedExtensions![0]);
        }
    }
}
