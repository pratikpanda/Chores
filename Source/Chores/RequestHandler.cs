using System.Threading;
using System.Threading.Tasks;

namespace Chores
{
    /// <summary>
    /// Wrapper class for a handler that synchronously handles a request and returns a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of request being handled.</typeparam>
    /// <typeparam name="TResponse">The type of response from the handler.</typeparam>
    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public abstract IRequestHandler<TRequest, TResponse> NextHandler { get; }

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}