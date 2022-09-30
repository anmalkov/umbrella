using System.Security.Cryptography.X509Certificates;
using Umbrella.Core.Models;

namespace Umbrella.Core.Services;

public interface ICoreService
{
    Task RegisterEntityAsync(IEntity entity, string extensionId);
}