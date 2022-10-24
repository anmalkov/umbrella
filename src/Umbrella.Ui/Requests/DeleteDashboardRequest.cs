namespace Umbrella.Ui.Requests;

public record struct DeleteDashboardRequest(
    string Id
) : IHttpRequest;
