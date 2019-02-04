using System.Threading;
using System.Threading.Tasks;

namespace Chores.Internal
{
    internal abstract class RequestHandlerWrapper<TResponse> : RequestHandlerBase
    {
        public abstract Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken, ServiceFactory serviceFactory);
    }
}