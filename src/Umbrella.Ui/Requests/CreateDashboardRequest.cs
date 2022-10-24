using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace Umbrella.Ui.Requests;

public record struct CreateDashboardRequestBody(
    string Name,
    IEnumerable<string> Widgets
);

public record struct CreateDashboardRequest(
    [FromBody]
    CreateDashboardRequestBody Body
) : IHttpRequest;
