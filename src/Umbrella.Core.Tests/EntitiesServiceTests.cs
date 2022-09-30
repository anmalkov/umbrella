using Umbrella.Core.Models;
using Umbrella.Core.Repositories;
using Umbrella.Core.Services;

namespace Umbrella.Core.Tests
{
    public class EntitiesServiceTests
    {
        [Fact]
        public async Task Register_throws_ArgumentExeption_for_dublicate_id()
        {
            var entity = new LightEntity("light.test");

            var repository = new Mock<IEntitiesRepository>();
            repository.Setup(r => r.GetAsync(It.IsAny<string>())).Returns(Task.FromResult(entity as IEntity));

            var service = new EntitiesService(repository.Object);
            
            await Assert.ThrowsAsync<ArgumentException>(async () => await service.RegisterAsync(entity));
        }
    }
}
