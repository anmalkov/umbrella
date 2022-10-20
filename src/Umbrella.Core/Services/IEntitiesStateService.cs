using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Models;

namespace Umbrella.Core.Services;

public interface IEntitiesStateService
{
    void StartMonitoring();
    void StopMonitoring();
    
    IEntityState? GetState(string id);
    IDictionary<string, IEntityState>? GetStates();
}
