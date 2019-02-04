using System;
using System.Collections.Generic;

namespace Chores
{
    /// <summary>
    /// Factory method used to resolve all services. For multiple instances, it will resolve against <see cref="IEnumerable{T}" />
    /// </summary>
    /// <param name="serviceType">Type of service to resolve</param>
    /// <returns>An instance of type <paramref name="serviceType" /></returns>
    public delegate object ServiceFactory(Type serviceType);

    public static class ServiceFactoryExtensions
    {
        public static T GetInstance<T>(this ServiceFactory serviceFactory) => (T)serviceFactory(typeof(T));

        public static IEnumerable<T> GetInstances<T>(this ServiceFactory serviceFactory) => (IEnumerable<T>)serviceFactory(typeof(IEnumerable<T>));
    }
}