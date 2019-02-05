using Chores.Chain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chores.Tests
{
    [TestClass]
    public class ChainSendTests
    {
        private ServiceProvider serviceProvider;

        public enum PingLevelTypes
        {
            Level0,
            Level1,
            Level2,
            Level3
        }

        [TestMethod]
        [TestCategory("Chain Send Tests")]
        public async Task Should_Resolve_Level0_Handler()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ServiceFactory>(sp => sp.GetService);

            services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(IChores), typeof(ChainSendTests))
            .AddClasses()
            .AsImplementedInterfaces());

            serviceProvider = services.BuildServiceProvider();
            var chores = serviceProvider.GetRequiredService<IChores>();
            var expected = "Ping Pong 0";

            // Act
            var response = await chores.SendToChain(new Ping { Message = "Ping", LevelType = PingLevelTypes.Level0 }, handlerType: typeof(Level0PingHandler));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Chain Send Tests")]
        public async Task Should_Resolve_Level1_Handler()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ServiceFactory>(sp => sp.GetService);

            services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(IChores), typeof(ChainSendTests))
            .AddClasses()
            .AsImplementedInterfaces());

            serviceProvider = services.BuildServiceProvider();
            var chores = serviceProvider.GetRequiredService<IChores>();
            var expected = "Ping Pong 1";

            // Act
            var response = await chores.SendToChain(new Ping { Message = "Ping", LevelType = PingLevelTypes.Level1 }, handlerType: typeof(Level0PingHandler));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Chain Send Tests")]
        public async Task Should_Resolve_Level2_Handler()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ServiceFactory>(sp => sp.GetService);

            services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(IChores), typeof(ChainSendTests))
            .AddClasses()
            .AsImplementedInterfaces());

            serviceProvider = services.BuildServiceProvider();
            var chores = serviceProvider.GetRequiredService<IChores>();
            var expected = "Ping Pong 2";

            // Act
            var response = await chores.SendToChain(new Ping { Message = "Ping", LevelType = PingLevelTypes.Level2 }, handlerType: typeof(Level0PingHandler));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Chain Send Tests")]
        public async Task Should_Resolve_Main_Handler()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ServiceFactory>(sp => sp.GetService);

            services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(IChores), typeof(ChainSendTests))
            .AddClasses()
            .AsImplementedInterfaces());

            serviceProvider = services.BuildServiceProvider();
            var chores = serviceProvider.GetRequiredService<IChores>();
            var expected = "Ping Pong";

            // Act
            var response = await chores.SendToChain(new Ping { Message = "Ping" }, typeof(PingHandler));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Chain Send Tests")]
        [ExpectedException(typeof(Exception))]
        public async Task Should_Throw_Exception()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ServiceFactory>(sp => sp.GetService);

            services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(IChores), typeof(ChainSendTests))
            .AddClasses()
            .AsImplementedInterfaces());

            serviceProvider = services.BuildServiceProvider();
            var chores = serviceProvider.GetRequiredService<IChores>();

            // Act
            var response = await chores.SendToChain(new Ping { Message = "Ping", LevelType = PingLevelTypes.Level3 }, handlerType: typeof(Level0PingHandler));
        }

        public class Level0PingHandler : RequestHandler<Ping, Pong>
        {
            public override IRequestHandler<Ping, Pong> NextHandler { get; } = new Level1PingHandler();

            public override async Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
            {
                if (request.LevelType == PingLevelTypes.Level0)
                {
                    return await Task.FromResult(new Pong() { Message = request.Message + " Pong" + " 0" });
                }
                else
                {
                    return await NextHandler.Handle(request, cancellationToken);
                }
            }
        }

        public class Level1PingHandler : RequestHandler<Ping, Pong>
        {
            public override IRequestHandler<Ping, Pong> NextHandler { get; } = new Level2PingHandler();

            public override async Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
            {
                if (request.LevelType <= PingLevelTypes.Level1)
                {
                    return await Task.FromResult(new Pong() { Message = request.Message + " Pong" + " 1" });
                }
                else
                {
                    return await NextHandler.Handle(request, cancellationToken);
                }
            }
        }

        public class Level2PingHandler : RequestHandler<Ping, Pong>
        {
            public override IRequestHandler<Ping, Pong> NextHandler { get; }

            public override async Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
            {
                if (request.LevelType <= PingLevelTypes.Level2)
                {
                    return await Task.FromResult(new Pong() { Message = request.Message + " Pong" + " 2" });
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public class Ping : IRequest<Pong>
        {
            public string Message { get; set; }
            public PingLevelTypes LevelType { get; set; }
        }

        public class PingHandler : IRequestHandler<Ping, Pong>
        {
            public IRequestHandler<Ping, Pong> NextHandler { get; }

            public Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Pong { Message = request.Message + " Pong" });
            }
        }

        public class Pong
        {
            public string Message { get; set; }
        }
    }
}