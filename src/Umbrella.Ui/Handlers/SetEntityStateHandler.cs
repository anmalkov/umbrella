using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using PhilipsHue.Sdk.Exceptions;
using System.Text.Json;
using Umbrella.Core.Events;
using Umbrella.Core.Models;
using Umbrella.Core.Services;
using Umbrella.Ui.Requests;

namespace Umbrella.Ui.Handlers;

public class SetEntityStateHandler : IRequestHandler<SetEntityStateRequest, IResult>
{
    private readonly IEntitiesService _entitiesService;
    private readonly IEventsService _eventsService;

    public SetEntityStateHandler(IEntitiesService entitiesService, IEventsService eventsService)
    {
        _entitiesService = entitiesService;
        _eventsService = eventsService;
    }
    
    public async Task<IResult> Handle(SetEntityStateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _entitiesService.GetAsync(request.Id);
            if (entity is null)
            {
                return Results.NotFound("Entity not found");
            }
            var state = ParseState(entity.Type, request.Body.State);
            if (state is null) {
                return Results.Problem("State is not correct");
            }
            _eventsService.Publish(new ChangeEntityStateEvent(entity.Id, state));
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private IEntityState? ParseState(EntityType type, object state)
    {
        switch (type)
        {
            case EntityType.Light:
                return JsonSerializer.Deserialize<LightEntityState>(JsonSerializer.Serialize(state), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            default:
                return default;
        }
    }
}
