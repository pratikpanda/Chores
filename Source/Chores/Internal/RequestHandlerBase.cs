using System;

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
    }
}