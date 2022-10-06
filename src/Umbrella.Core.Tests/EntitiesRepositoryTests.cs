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
                new LightEntity("light.test1") { Name = "Light 1", Available = true, MinColorTemperature = 100, MaxColorTemperature = 5000 },
                new LightEntity("light.test2") { Name = "Light 2", Available = true, MinColorTemperature = 3000, MaxColorTemperature = 6000 }
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

            foreach (var entity in entities)
            {
                var loadedEntity = await repository.GetAsync(entity.Id);
                Assert.NotNull(loadedEntity);
                Assert.Equivalent(entity, loadedEntity);
            }
        }

        [Fact]
        public async Task Get_returns_entity_by_id()
        {
            var path = "./unit-tests";
            var fullFilename = Path.Combine(path, RepositoryFilename);

            var entities = new List<IEntity>
            {
                new LightEntity("light.test1") { Name = "Light 1", MinColorTemperature = 100, MaxColorTemperature = 5000 },
                new LightEntity("light.test2") { Name = "Light 2", MinColorTemperature = 3000, MaxColorTemperature = 6000 }
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

        [Fact]
        public async Task Delete_removes_entity()
        {
            var path = "./unit-tests";
            var fullFilename = Path.Combine(path, RepositoryFilename);

            var entities = new List<IEntity>
            {
                new LightEntity("light.test1") { Name = "Light 1", MinColorTemperature = 100, MaxColorTemperature = 5000 },
                new LightEntity("light.test2") { Name = "Light 2", MinColorTemperature = 3000, MaxColorTemperature = 6000 }
            };

            if (File.Exists(fullFilename))
            {
                File.Delete(fullFilename);
            }

            var config = new Mock<IConfigurationRepository>();
            config.SetupGet(x => x.RepositoriesDirectory).Returns(path);

            var repository = new EntitiesRepository(config.Object);
            await repository.AddAsync(entities);

            await repository.DeleteAsync("light.test1");

            var loadedEntities = await repository.GetAllAsync();
                        
            Assert.NotNull(loadedEntities);
            Assert.Single(loadedEntities!);
            Assert.Equivalent(entities[1], loadedEntities[0] as LightEntity);
        }

        [Fact]
        public async Task DeleteByOwner_removes_entities_that_has_an_owner()
        {
            var path = "./unit-tests";
            var fullFilename = Path.Combine(path, RepositoryFilename);

            var entities = new List<IEntity>
            {
                new LightEntity("light.test1") { Name = "Light 1", MinColorTemperature = 100, MaxColorTemperature = 5000, Owner = "another" },
                new LightEntity("light.test2") { Name = "Light 2", MinColorTemperature = 200, MaxColorTemperature = 5100, Owner = "test" },
                new LightEntity("light.test3") { Name = "Light 3", MinColorTemperature = 300, MaxColorTemperature = 5200, Owner = "another" },
                new LightEntity("light.test4") { Name = "Light 4", MinColorTemperature = 400, MaxColorTemperature = 5300, Owner = "test" },
                new LightEntity("light.test5") { Name = "Light 5", MinColorTemperature = 500, MaxColorTemperature = 5400, Owner = "another" },
                new LightEntity("light.test6") { Name = "Light 6", MinColorTemperature = 600, MaxColorTemperature = 5500, Owner = "test" }
            };

            if (File.Exists(fullFilename))
            {
                File.Delete(fullFilename);
            }

            var config = new Mock<IConfigurationRepository>();
            config.SetupGet(x => x.RepositoriesDirectory).Returns(path);

            var repository = new EntitiesRepository(config.Object);
            await repository.AddAsync(entities);

            await repository.DeleteByOwnerAsync("another");

            var loadedEntities = await repository.GetAllAsync();

            Assert.NotNull(loadedEntities);
            Assert.Equal(3, loadedEntities!.Count);
            foreach (var entity in entities.Where(e => e.Owner == "test"))
            {
                var loadedEntity = await repository.GetAsync(entity.Id);
                Assert.NotNull(loadedEntity);
                Assert.Equivalent(entity, loadedEntity);
            }
        }
    }
}
