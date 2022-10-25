using MediatR;
using Umbrella.Core.Services;
using Umbrella.Ui.Requests;

namespace Umbrella.Ui.Handlers;

public class GetDashboardsHandler : IRequestHandler<GetDashboardsRequest, IResult>
{
    private readonly IDashboardsService _dashboardsService;

    public record WidgetDto(
        int Id,
        string? Name,
        byte Column,
        byte PositionInColumn,
        string Type,
        IEnumerable<string> TargetIds);
    
    public record DashboardDto(
        string Id,
        string Name,
        string? Icon,
        IEnumerable<WidgetDto> Widgets);
    
    public GetDashboardsHandler(IDashboardsService dashboardsService)
    {
        _dashboardsService = dashboardsService;
    }

    public async Task<IResult> Handle(GetDashboardsRequest request, CancellationToken cancellationToken)
    {
        var dashboards = (await _dashboardsService.GetAllAsync())?.Select(d => new DashboardDto
        (
            d.Id,
            d.Name,
            d.Icon,
            d.Widgets.Select(w => new WidgetDto(
                w.Id,
                w.Name,
                w.Column,
                w.PositionInColumn,
                w.Type,
                w.TargetIds
            ))
        ));
        return Results.Ok(dashboards);
    }
}
