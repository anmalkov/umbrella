using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using PhilipsHue.Sdk.Exceptions;
using Umbrella.Core.Services;
using Umbrella.Ui.Requests;

namespace Umbrella.Ui.Handlers;

public class UnregisterExtensionHandler : IRequestHandler<UnregisterExtensionRequest, IResult>
{
    private readonly IExtensionsService _extensionsService;

    public UnregisterExtensionHandler(IExtensionsService extensionsService)
    {
        _extensionsService = extensionsService;
    }
    
    public async Task<IResult> Handle(UnregisterExtensionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _extensionsService.UnregisterAsync(request.Id);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}
