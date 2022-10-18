using MediatR;
using Umbrella.Core.Services;
using Umbrella.Ui.Requests;

namespace Umbrella.Ui.Handlers;

public class GetAreasHandler : IRequestHandler<GetAreasRequest, IResult>
{
    private readonly IAreasService _areasService;

    public record AreaDto(string Id, string Name, string? Icon, double? Longitude, double? Latitude, bool InsideHouse);
    
    public GetAreasHandler(IAreasService areasService)
    {
        _areasService = areasService;
    }

    public async Task<IResult> Handle(GetAreasRequest request, CancellationToken cancellationToken)
    {
        var areas = (await _areasService.GetAllAsync())?.Select(a => new AreaDto
        (
            a.Id,
            a.Name,
            a.Icon,
            a.Longitude,
            a.Latitude,
            a.InsideHouse
        ));
        return Results.Ok(areas);
    }
}
