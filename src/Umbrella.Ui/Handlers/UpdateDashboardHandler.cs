using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using PhilipsHue.Sdk.Exceptions;
using Umbrella.Core.Models;
using Umbrella.Core.Services;
using Umbrella.Ui.Requests;

namespace Umbrella.Ui.Handlers;

public class UpdateDashboardHandler : IRequestHandler<UpdateDashboardRequest, IResult>
{
    private readonly IDashboardsService _dashboardsService;

    public UpdateDashboardHandler(IDashboardsService dashboardsService)
    {
        _dashboardsService = dashboardsService;
    }
    
    public async Task<IResult> Handle(UpdateDashboardRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _dashboardsService.UpdateAsync(MapRequestToDashboard(request));
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private Dashboard MapRequestToDashboard(UpdateDashboardRequest request)
    {
        return new Dashboard(request.Id, request.Body.Name)
        {
            Widgets = request.Body.Widgets.ToList()
        };

    }
}
