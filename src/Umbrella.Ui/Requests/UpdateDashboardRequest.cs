using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Umbrella.Core.Models;

namespace Umbrella.Ui.Requests;

public record struct UpdateDashboardRequestBody(
    string Name,
    IEnumerable<Widget> Widgets
);

public record struct UpdateDashboardRequest(
    string Id,
    [FromBody]
    UpdateDashboardRequestBody Body
) : IHttpRequest;
