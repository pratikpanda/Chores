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
        public override Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken, ServiceFactory serviceFactory)
        {
            Task<TResponse> Handler() => GetHandler<IRequestHandler<TRequest, TResponse>>(serviceFactory).Handle((TRequest)request, cancellationToken);

            return serviceFactory
                .GetInstances<IPipelineBehavior<TRequest, TResponse>>()
                .Reverse()
                .Aggregate((RequestHandlerDelegate<TResponse>)Handler, (next, pipeline) => () => pipeline.Handle((TRequest)request, cancellationToken, next))();
        }
    }
}
