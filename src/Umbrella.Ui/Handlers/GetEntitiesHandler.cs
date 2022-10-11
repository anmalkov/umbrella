using MediatR;
using Umbrella.Core.Services;
using Umbrella.Ui.Requests;

namespace Umbrella.Ui.Handlers;

public class GetEntitiesHandler : IRequestHandler<GetEntitiesRequest, IResult>
{
    private readonly IEntitiesService _entitiesService;

    public record EntityDto(string Id, string? Name, string? Icon, string? Owner, bool Enabled, string Type);

    public GetEntitiesHandler(IEntitiesService entitiesService)
    {
        _entitiesService = entitiesService;
    }

    public async Task<IResult> Handle(GetEntitiesRequest request, CancellationToken cancellationToken)
    {
        var entities = (await _entitiesService.GetAllAsync())?.Select(e => new EntityDto(
            e.Id,
            e.Name,
            e.Icon,
            e.Owner,
            e.Enabled,
            e.Type.ToString()
        ));
        return Results.Ok(entities);
    }
}
