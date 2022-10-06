using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Events;

public static class EventNames
{
    //public const string EntityRegistered = "EntityRegistered";
    //public const string EntityUpdated = "EntityUpdated";
    //public const string EntityDeleted = "EntityDeleted";
    //public const string EntityDeletedByOwner = "EntityDeletedByOwner";
    //public const string ExtensionRegistered = "ExtensionRegistered";
    //public const string ExtensionUnregistered = "ExtensionUnregistered";
    //public const string ExtensionStarted = "ExtensionStarted";
    //public const string ExtensionStopped = "ExtensionStopped";

    public const string ChangeEntityState = "entity.change_state";
    public const string EntityStateChanged = "entity.state_changed";
}
