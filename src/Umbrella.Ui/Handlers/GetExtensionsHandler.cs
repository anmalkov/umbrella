using MediatR;
using Umbrella.Core.Services;
using Umbrella.Ui.Requests;

namespace Umbrella.Ui.Handlers;

public class GetExtensionsHandler : IRequestHandler<GetExtensionsRequest, IResult>
{
    private readonly IExtensionsService _extensionsService;
    private readonly IEntitiesService _entitiesService;

    public record ExtensionDto(string Id, string? DisplayName, string? Image, string? HtmlForRegistration, bool Registered, int EntitiesCount);

    public GetExtensionsHandler(IExtensionsService extensionsService, IEntitiesService entitiesService)
    {
        _extensionsService = extensionsService;
        _entitiesService = entitiesService;
    }

    public async Task<IResult> Handle(GetExtensionsRequest request, CancellationToken cancellationToken)
    {
        var registeredExtensions = await _extensionsService.GetRegisteredAsync();
        var extensions = new List<ExtensionDto>();
        foreach (var extension in await _extensionsService.GetAllAsync())
        {
            var registered = registeredExtensions.Any(r => r.Id == extension.Id);
            var entitiesCount = registered ? await _entitiesService.GetCount(extension.Id) : 0;
            extensions.Add(new ExtensionDto(
                extension.Id,
                extension.DisplayName,
                extension.Image,
                extension.HtmlForRegistration,
                registered,
                entitiesCount
                )
            );
        }
        return Results.Ok(extensions);
    }
}
