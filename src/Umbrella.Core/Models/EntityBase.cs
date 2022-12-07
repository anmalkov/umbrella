namespace Umbrella.Core.Models;

public enum EntityType
{
    Light = 1,
    Weather = 2,
}

public class EntityBase : IEntity
{
    public string Id {get; init;}
    public string? Name { get; set; }
    public string? Icon { get; set; }
    public string? Owner { get; set; }
    public string? AreaId { get; set; }
    public bool Enabled { get; set; }
    public EntityType Type { get; init; }

    public EntityBase(string id, EntityType type)
    {
        Id = id;
        Type = type;
    }
}
