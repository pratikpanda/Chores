using Chores.Internal;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Chores
{
    public class Chores : IChores
    {
        private static readonly ConcurrentDictionary<Type, object> requestHandlers = new ConcurrentDictionary<Type, object>();
        private readonly ServiceFactory serviceFactory;

        public Chores(ServiceFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory;
        }

        public Task<TResponse> SendToChain<TResponse>(IRequest<TResponse> request, Type handlerType, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestType = request.GetType();
            var handler = (RequestHandlerWrapper<TResponse>)requestHandlers.GetOrAdd(requestType,
                    t => Activator.CreateInstance(typeof(RequestHandlerWrapperImplementation<,>).MakeGenericType(requestType, typeof(TResponse))));

            if (handlerType == null)
            {
                return handler.HandleChain(request, cancellationToken, serviceFactory);
            }
            else
            {
                return handler.HandleChain(request, cancellationToken, serviceFactory, handlerType);
            }
        }

        public Task<TResponse> SendToTree<TResponse>(IRequest<TResponse> request, Type handlerType, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestType = request.GetType();
            var handler = (RequestHandlerWrapper<TResponse>)requestHandlers.GetOrAdd(requestType,
                    t => Activator.CreateInstance(typeof(RequestHandlerWrapperImplementation<,>).MakeGenericType(requestType, typeof(TResponse))));

            if (handlerType == null)
            {
                return handler.HandleTree(request, cancellationToken, serviceFactory);
            }
            else
            {
                return handler.HandleTree(request, cancellationToken, serviceFactory, handlerType);
            }
        }
    }
}