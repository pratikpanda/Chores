using Microsoft.Extensions.DependencyInjection;
using System;

namespace Chores.Samples.NetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var chores = BuildChores();
            var pong = chores.Send(new Ping { Message = "ping" }).Result;
            Console.ReadKey();
        }

        private static IChores BuildChores()
        {
            var services = new ServiceCollection();
            services.AddScoped<ServiceFactory>(sp => sp.GetService);

            // Pipeline
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(GenericPipelineBehavior<,>));

            services.Scan(scan => scan
            .FromAssembliesOf(typeof(IChores), typeof(Ping))
            .AddClasses()
            .AsImplementedInterfaces());

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IChores>();
        }
    }
}
