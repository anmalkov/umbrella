using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Models;

namespace Umbrella.Core.Extensions;

public static class ExtensionsHelper
{
    public static string GenerateEntityId(string extensionId, EntityType type, string name)
    {
        name = name.ToLower().Replace(' ', '_');
        
        var prefix = type switch
        {
            EntityType.Light => "light",
            EntityType.Weather => "weather",
            EntityType.Temperature => "temperature",
            EntityType.Binary => "binary",
            _ => "unknown"
        };

        return $"{prefix}.{extensionId}.{name}";
    }

    public static string? GetParameterValue(Dictionary<string, string?>? parameters, string parameterName, bool parameterRequired)
    {
        if (parameters is null || !parameters.ContainsKey(parameterName))
        {
            if (!parameterRequired)
            {
                return null;
            }
            throw new ArgumentException($"Missing required parameter '{parameterName}'");
        }

        var value = parameters[parameterName];
        if (parameterRequired && string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Parameter '{parameterName}' must have a value");
        }

        return value;
    }
}
