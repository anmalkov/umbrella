using Umbrella.Core.Models;
using Umbrella.Core.Repositories;
using Umbrella.Core.Services;

namespace Umbrella.Core.Tests
{
    public class CoreServiceTests
    {
        [Fact]
        public async Task RegisterEntity_should_assign_owner()
        {
            var extensionId = "extension";
            
            var entity = new LightEntity("light.test") { Owner = "malicius" };

            var entitiesService = new Mock<IEntitiesService>();
            
            var service = new CoreService(entitiesService.Object);
            await service.RegisterEntityAsync(entity, extensionId);

            entitiesService.Verify(s => s.RegisterAsync(It.Is<IEntity>(e => e.Owner == extensionId)));
        }
    }
}
