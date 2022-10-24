using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace Umbrella.Ui.Requests;

public record struct UpdateDashboardRequestBody(
    string Name,
    IEnumerable<string> Widgets
);

public record struct UpdateDashboardRequest(
    string Id,
    [FromBody]
    UpdateDashboardRequestBody Body
) : IHttpRequest;
