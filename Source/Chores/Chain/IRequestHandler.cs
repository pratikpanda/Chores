using System.Threading;
using System.Threading.Tasks;

namespace Chores.Chain
{
    /// <summary>
    /// Defines a handler for the request.
    /// </summary>
    /// <typeparam name="TRequest">The type of request being handled.</typeparam>
    /// <typeparam name="TResponse">The type of response from the handler.</typeparam>
    public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// The next handler in the chain to handle the request.
        /// </summary>
        IRequestHandler<TRequest, TResponse> NextHandler { get; }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Response from the request.</returns>
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}