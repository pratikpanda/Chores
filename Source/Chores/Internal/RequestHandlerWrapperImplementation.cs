using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chores.Internal
{
    internal class RequestHandlerWrapperImplementation<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
        where TRequest : IRequest<TResponse>
    {
        public override Task<TResponse> HandleChain(IRequest<TResponse> request, CancellationToken cancellationToken, ServiceFactory serviceFactory, Type handlerType = null)
        {
            if (handlerType == null)
            {
                Task<TResponse> Handler() => GetHandler<Chain.IRequestHandler<TRequest, TResponse>>(serviceFactory).Handle((TRequest)request, cancellationToken);
                return serviceFactory
                .GetInstances<IPipelineBehavior<TRequest, TResponse>>()
                .Reverse()
                .Aggregate((RequestHandlerDelegate<TResponse>)Handler, (next, pipeline) => () => pipeline.Handle((TRequest)request, cancellationToken, next))();
            }
            else
            {
                Task<TResponse> Handler() => GetHandlers<Chain.IRequestHandler<TRequest, TResponse>>(serviceFactory).FirstOrDefault(h => h.GetType() == handlerType).Handle((TRequest)request, cancellationToken);
                return serviceFactory
                .GetInstances<IPipelineBehavior<TRequest, TResponse>>()
                .Reverse()
                .Aggregate((RequestHandlerDelegate<TResponse>)Handler, (next, pipeline) => () => pipeline.Handle((TRequest)request, cancellationToken, next))();
            }
        }

        public override Task<TResponse> HandleTree(IRequest<TResponse> request, CancellationToken cancellationToken, ServiceFactory serviceFactory, Type handlerType = null)
        {
            if (handlerType == null)
            {
                Task<TResponse> Handler() => GetHandler<Tree.IRequestHandler<TRequest, TResponse>>(serviceFactory).Handle((TRequest)request, cancellationToken);
                return serviceFactory
                .GetInstances<IPipelineBehavior<TRequest, TResponse>>()
                .Reverse()
                .Aggregate((RequestHandlerDelegate<TResponse>)Handler, (next, pipeline) => () => pipeline.Handle((TRequest)request, cancellationToken, next))();
            }
            else
            {
                Task<TResponse> Handler() => GetHandlers<Tree.IRequestHandler<TRequest, TResponse>>(serviceFactory).FirstOrDefault(h => h.GetType() == handlerType).Handle((TRequest)request, cancellationToken);
                return serviceFactory
                .GetInstances<IPipelineBehavior<TRequest, TResponse>>()
                .Reverse()
                .Aggregate((RequestHandlerDelegate<TResponse>)Handler, (next, pipeline) => () => pipeline.Handle((TRequest)request, cancellationToken, next))();
            }
        }
    }
}
