using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using PhilipsHue.Sdk.Exceptions;
using Umbrella.Core.Models;
using Umbrella.Core.Services;
using Umbrella.Ui.Requests;

namespace Umbrella.Ui.Handlers;

public class CreateDashboardHandler : IRequestHandler<CreateDashboardRequest, IResult>
{
    private readonly IDashboardsService _dashboardsService;

    public CreateDashboardHandler(IDashboardsService dashboardsService)
    {
        _dashboardsService = dashboardsService;
    }
    
    public async Task<IResult> Handle(CreateDashboardRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _dashboardsService.AddAsync(MapRequestToDashboard(request.Body));
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private Dashboard MapRequestToDashboard(CreateDashboardRequestBody request)
    {
        var id = $"dashboard.{request.Name.ToLower().Replace(' ', '_')}";
        return new Dashboard(id, request.Name);
    }
}
