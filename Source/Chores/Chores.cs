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

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestType = request.GetType();
            var handler = (RequestHandlerWrapper<TResponse>)requestHandlers.GetOrAdd(requestType,
                t => Activator.CreateInstance(typeof(RequestHandlerWrapperImplementation<,>).MakeGenericType(requestType, typeof(TResponse))));

            return handler.Handle(request, cancellationToken, serviceFactory);
        }
    }
}