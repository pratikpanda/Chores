using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chores.Internal
{
    internal abstract class RequestHandlerWrapper<TResponse> : RequestHandlerBase
    {
        public abstract Task<TResponse> HandleChain(IRequest<TResponse> request, CancellationToken cancellationToken, ServiceFactory serviceFactory, Type handlerType = null);
        public abstract Task<TResponse> HandleTree(IRequest<TResponse> request, CancellationToken cancellationToken, ServiceFactory serviceFactory, Type handlerType = null);
    }
}