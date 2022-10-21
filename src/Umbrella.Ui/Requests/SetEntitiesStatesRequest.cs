using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Umbrella.Core.Models;
using Umbrella.Core.Services;

namespace Umbrella.Ui.Requests;

public record struct SetEntitiesStatesRequestBody(
    IEnumerable<KeyValuePair<string, object>>? States
);

public record struct SetEntitiesStatesRequest(
    [FromBody]
    SetEntitiesStatesRequestBody Body
) : IHttpRequest;
