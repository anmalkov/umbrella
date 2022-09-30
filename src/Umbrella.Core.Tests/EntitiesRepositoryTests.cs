using Umbrella.Core.Models;
using Umbrella.Core.Repositories;

namespace Umbrella.Core.Tests
{
    public class EntitiesRepositoryTests
    {
        private const string RepositoryFilename = "entities.json";

        [Fact]
        public async Task Add_creates_file()
        {
            var path = "./unit-tests";
            var fullFilename = Path.Combine(path, RepositoryFilename);

            var entity = new LightEntity("light.test");

            if (File.Exists(fullFilename))
            {
                File.Delete(fullFilename);
            }

            var config = new Mock<IConfigurationRepository>();
            config.SetupGet(x => x.RepositoriesDirectory).Returns(path);

            var repository = new EntitiesRepository(config.Object);
            await repository.AddAsync(entity);

            Assert.True(File.Exists(fullFilename));
        }

        [Fact]
        public async Task GetAll_returns_all_entities()
        {
            var path = "./unit-tests";
            var fullFilename = Path.Combine(path, RepositoryFilename);

            var entities = new List<IEntity>
            {
                new LightEntity("light.test1") { Name = "Light 1", TurnedOn = true, Available = true, Brightness = 100, RgbColor = new(1, 2, 3) },
                new LightEntity("light.test2") { Name = "Light 2", TurnedOn = false, Available = true, Brightness = 50 }
            };

            if (File.Exists(fullFilename))
            {
                File.Delete(fullFilename);
            }

            var config = new Mock<IConfigurationRepository>();
            config.SetupGet(x => x.RepositoriesDirectory).Returns(path);

            var repository = new EntitiesRepository(config.Object);
            await repository.AddAsync(entities);

            var loadedEntities = await repository.GetAllAsync();

            Assert.Equal(entities.Count, loadedEntities?.Count);
            
            var lightEntity = loadedEntities?.First(e => e.Id == entities[0].Id) as LightEntity;
            Assert.Equivalent(entities[0], lightEntity);
            lightEntity = loadedEntities?.First(e => e.Id == entities[1].Id) as LightEntity;
            Assert.Equivalent(entities[1], lightEntity);
        }

        [Fact]
        public async Task Get_returns_entity_by_id()
        {
            var path = "./unit-tests";
            var fullFilename = Path.Combine(path, RepositoryFilename);

            var entities = new List<IEntity>
            {
                new LightEntity("light.test1") { Name = "Light 1", TurnedOn = true, Available = true, Brightness = 100 },
                new LightEntity("light.test2") { Name = "Light 2", TurnedOn = false, Available = true, Brightness = 50 }
            };

            if (File.Exists(fullFilename))
            {
                File.Delete(fullFilename);
            }

            var config = new Mock<IConfigurationRepository>();
            config.SetupGet(x => x.RepositoriesDirectory).Returns(path);

            var repository = new EntitiesRepository(config.Object);
            await repository.AddAsync(entities);

            foreach (var entity in entities)
            {
                var loadedEntity = await repository.GetAsync(entity.Id);
                Assert.NotNull(loadedEntity);
                Assert.Equivalent(entity, loadedEntity);

            }
        }
    }
}
