using MediatR;
using System.Text.Json;
using Umbrella.Core.Models;
using Umbrella.Core.Services;
using Umbrella.Ui.Requests;

namespace Umbrella.Ui.Handlers;

public class GetEntitiesStatesHandler : IRequestHandler<GetEntitiesStatesRequest, IResult>
{
    private readonly IEntitiesService _entitiesService;

    public record EntityStateDto(string Id, object state);

    public GetEntitiesStatesHandler(IEntitiesService entitiesService)
    {
        _entitiesService = entitiesService;
    }

    public Task<IResult> Handle(GetEntitiesStatesRequest request, CancellationToken cancellationToken)
    {
        var states = _entitiesService.GetStates()?.Select(e => new EntityStateDto(e.Key, e.Value));
        return Task.FromResult(Results.Ok(states));
    }
}
