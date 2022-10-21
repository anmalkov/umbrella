using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Umbrella.Core.Models;
using Umbrella.Core.Services;

namespace Umbrella.Ui.Requests;

public record struct SetEntityStateRequestBody(
    object State
);

public record struct SetEntityStateRequest(
    string Id,
    [FromBody]
    SetEntityStateRequestBody Body
) : IHttpRequest;
