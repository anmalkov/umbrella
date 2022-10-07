namespace Umbrella.Core.Models;

public interface IEntityState
{
    bool? Connected { get; set; }

    IEntityState Clone();
    void UpdateProperties(IEntityState state);
}