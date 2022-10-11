using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace Umbrella.Ui.Requests;

public record struct RegisterExtensionRequestBody(
    IEnumerable<KeyValuePair<string,string?>>? Parameters
);

public record struct RegisterExtensionRequest(
    string Id,
    [FromBody]
    RegisterExtensionRequestBody Body
) : IHttpRequest;
