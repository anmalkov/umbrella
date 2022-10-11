namespace Umbrella.Ui.Requests;

public record struct UnregisterExtensionRequest(
    string Id
) : IHttpRequest;
