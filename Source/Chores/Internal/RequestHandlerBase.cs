using System;
using System.Collections.Generic;
using System.Linq;

namespace Chores.Internal
{
    internal abstract class RequestHandlerBase
    {
        protected static THandler GetHandler<THandler>(ServiceFactory serviceFactory)
        {
            THandler handler;

            try
            {
                handler = serviceFactory.GetInstance<THandler>();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error constructing handler for request of type {typeof(THandler)}. Register your handlers with the container. See the samples in GitHub for examples.", e);
            }

            if (handler == null)
            {
                throw new InvalidOperationException($"Handler was not found for request of type {typeof(THandler)}. Register your handlers with the container. See the samples in GitHub for examples.");
            }

            return handler;
        }

        protected static IEnumerable<THandler> GetHandlers<THandler>(ServiceFactory serviceFactory)
        {
            List<THandler> handlers;

            try
            {
                handlers = serviceFactory.GetInstances<THandler>().ToList();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error constructing handler for request of type {typeof(THandler)}. Register your handlers with the container. See the samples in GitHub for examples.", e);
            }

            if (handlers.Count == 0)
            {
                throw new InvalidOperationException($"Handler was not found for request of type {typeof(THandler)}. Register your handlers with the container. See the samples in GitHub for examples.");
            }

            return handlers;
        }
    }
}