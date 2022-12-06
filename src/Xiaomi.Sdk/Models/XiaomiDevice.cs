using System.Text.Json.Serialization;

namespace Xiaomi.Sdk.Models;

public record XiaomiDevice(
    string Id,
    string Model,
    IDictionary<string, string> Properties
);
