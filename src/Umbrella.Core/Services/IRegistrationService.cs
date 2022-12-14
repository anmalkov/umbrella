using System.Security.Cryptography.X509Certificates;
using Umbrella.Core.Models;

namespace Umbrella.Core.Services;

public interface IRegistrationService
{
    Task<IEntity?> GetEntityAsync(string id, string extensionId);
    Task RegisterEntityAsync(IEntity entity, string extensionId);
    Task AddAreaAsync(Area area, string extensionId);
    Task AddGroupAsync(Group group, string extensionId);
}