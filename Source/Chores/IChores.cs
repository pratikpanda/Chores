using System;
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
        /// Asynchronously send a request to the first handler of the chain.
        /// </summary>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <param name="handlerType">Type of handler object.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A task that represents the send operation. The task result contains the handler response.</returns>
        Task<TResponse> SendToChain<TResponse>(IRequest<TResponse> request, Type handlerType, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously send a request to the first handler of the tree.
        /// </summary>
        /// <typeparam name="TResponse">Response type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <param name="handlerType">Type of handler object.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A task that represents the send operation. The task result contains the handler response.</returns>
        Task<TResponse> SendToTree<TResponse>(IRequest<TResponse> request, Type handlerType, CancellationToken cancellationToken = default(CancellationToken));
    }
}