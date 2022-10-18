using MediatR;
using Umbrella.Core.Services;
using Umbrella.Ui.Requests;

namespace Umbrella.Ui.Handlers;

public class GetGroupsHandler : IRequestHandler<GetGroupsRequest, IResult>
{
    private readonly IGroupsService _groupsService;

    public record GroupDto(string Id, string Name, string? Icon, List<string> Entities);
    
    public GetGroupsHandler(IGroupsService groupsService)
    {
        _groupsService = groupsService;
    }

    public async Task<IResult> Handle(GetGroupsRequest request, CancellationToken cancellationToken)
    {
        var groups = (await _groupsService.GetAllAsync())?.Select(g => new GroupDto
        (
            g.Id,
            g.Name,
            g.Icon,
            g.Entities
        ));
        return Results.Ok(groups);
    }
}
