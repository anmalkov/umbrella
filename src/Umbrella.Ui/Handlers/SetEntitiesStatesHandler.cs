using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using PhilipsHue.Sdk.Exceptions;
using System.Text;
using System.Text.Json;
using Umbrella.Core.Events;
using Umbrella.Core.Models;
using Umbrella.Core.Services;
using Umbrella.Ui.Requests;

namespace Umbrella.Ui.Handlers;

public class SetEntitiesStatesHandler : IRequestHandler<SetEntitiesStatesRequest, IResult>
{
    private readonly IEntitiesService _entitiesService;
    private readonly IEventsService _eventsService;

    public SetEntitiesStatesHandler(IEntitiesService entitiesService, IEventsService eventsService)
    {
        _entitiesService = entitiesService;
        _eventsService = eventsService;
    }
    
    public async Task<IResult> Handle(SetEntitiesStatesRequest request, CancellationToken cancellationToken)
    {
        var errors = new StringBuilder();
        try
        {
            foreach (var bodyState in request.Body.States)
            {
                var entity = await _entitiesService.GetAsync(bodyState.Key);
                if (entity is null)
                {
                    errors.Append($"Entity {bodyState.Key} not found");
                    continue;
                }
                var state = ParseState(entity.Type, bodyState.Value);
                if (state is null)
                {
                    errors.Append($"State for entity {bodyState.Key} is not correct");
                    continue;
                }
                _eventsService.Publish(new ChangeEntityStateEvent(entity.Id, state));
            }
            if (errors.Length > 0)
            {
                return Results.Problem(errors.ToString());
            }
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
