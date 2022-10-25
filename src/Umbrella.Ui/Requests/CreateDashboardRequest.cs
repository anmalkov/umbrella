using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Umbrella.Core.Models;

namespace Umbrella.Ui.Requests;

public record struct CreateDashboardRequestBody(
    string Name,
    IEnumerable<Widget> Widgets
);

public record struct CreateDashboardRequest(
    [FromBody]
    CreateDashboardRequestBody Body
) : IHttpRequest;
