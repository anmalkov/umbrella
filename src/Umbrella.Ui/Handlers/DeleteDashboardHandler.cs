using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using PhilipsHue.Sdk.Exceptions;
using Umbrella.Core.Models;
using Umbrella.Core.Services;
using Umbrella.Ui.Requests;

namespace Umbrella.Ui.Handlers;

public class DeleteDashboardHandler : IRequestHandler<DeleteDashboardRequest, IResult>
{
    private readonly IDashboardsService _dashboardsService;

    public DeleteDashboardHandler(IDashboardsService dashboardsService)
    {
        _dashboardsService = dashboardsService;
    }
    
    public async Task<IResult> Handle(DeleteDashboardRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _dashboardsService.DeleteAsync(request.Id);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}
