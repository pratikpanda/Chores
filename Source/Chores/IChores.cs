using System.Threading;
using System.Threading.Tasks;

namespace Chores
{
    /// <summary>
    /// Defines a component to encapsulate request/response through chain of responsibility pattern.
    /// </summary>
    public interface IChores
    {
        /// <summary>
        /// Asynchronously send a request to the first handler.
        /// </summary>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A task that represents the send operation. The task result contains the handler response.</returns>
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken));
    }
}