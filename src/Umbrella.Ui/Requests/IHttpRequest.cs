using MediatR;

namespace Umbrella.Ui.Requests;

public interface IHttpRequest : IRequest<IResult> { }
