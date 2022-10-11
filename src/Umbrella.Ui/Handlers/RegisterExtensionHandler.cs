using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using PhilipsHue.Sdk.Exceptions;
using Umbrella.Core.Services;
using Umbrella.Ui.Requests;

namespace Umbrella.Ui.Handlers;

public class RegisterExtensionHandler : IRequestHandler<RegisterExtensionRequest, IResult>
{
    private readonly IExtensionsService _extensionsService;

    public RegisterExtensionHandler(IExtensionsService extensionsService)
    {
        _extensionsService = extensionsService;
    }
    
    public async Task<IResult> Handle(RegisterExtensionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _extensionsService.RegisterAsync(request.Id, request.Body.Parameters);
            return Results.Ok();
        }
        catch (ArgumentException ex)
        {
            return Results.Problem(ex.Message, statusCode: 400);
        }
        catch (PhilipsHueException ex)
        {
            return Results.Problem(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}
