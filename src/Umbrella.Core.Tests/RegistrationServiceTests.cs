using Umbrella.Core.Events;
using Umbrella.Core.Models;
using Umbrella.Core.Repositories;
using Umbrella.Core.Services;

namespace Umbrella.Core.Tests;

public class RegistrationServiceTests
{
    [Fact]
    public async Task RegisterEntity_should_assign_owner()
    {
        var extensionId = "extension";

        var entity = new LightEntity("light.test") { Owner = "malicius" };

        var entitiesService = new Mock<IEntitiesService>();

        var service = new RegistrationService(entitiesService.Object, null, null);
        await service.RegisterEntityAsync(entity, extensionId);

        entitiesService.Verify(s => s.RegisterAsync(It.Is<IEntity>(e => e.Owner == extensionId)));
    }

    //[Fact]
    //public async Task test()
    //{
    //    IEvent myEvent = new EntityStateChangedEvent<LightEntityState>("testId", new LightEntityState());

    //    var type = myEvent.GetType();
    //    var ismy = type.IsGenericType && type.GetGenericArguments().All(e => e.GetInterface(nameof(IEntityState)) is not null);
    //    var gtypeDef = type.GetGenericTypeDefinition();
    //    var interfaces = type.GetInterfaces();

    //    Assert.True(myEvent is IEvent);
    //    Assert.True(myEvent is EntityStateChangedEvent<LightEntityState>);
    //    Assert.True(myEvent.GetType() == typeof(EntityStateChangedEvent<>));
    //    Assert.True(myEvent is EntityStateChangedEvent<IEntityState>);

        
            
    //    //bool result1 = myEvent.GetType().GetInterfaces()
    //    //    .Where(i => i.IsGenericType)
    //    //    .Any(i => i.GetGenericTypeDefinition() == typeof(IEntityState));
    //}
}
