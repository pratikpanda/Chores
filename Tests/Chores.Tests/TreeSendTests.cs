using Chores.Tree;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chores.Tests
{
    [TestClass]
    public class TreeSendTests
    {
        private ServiceProvider serviceProvider;

        public enum PingLevelTypes
        {
            Level0,
            Level1Alternate,
            Level1AnotherAlternate,
            Level1,
            Level2,
            Level3
        }

        public class Pong
        {
            public string Message { get; set; }
        }

        public class Ping : IRequest<Pong>
        {
            public string Message { get; set; }
            public PingLevelTypes LevelType { get; set; }
        }

        public class PingHandler : IRequestHandler<Ping, Pong>
        {
            public ConcurrentDictionary<Type, object> NextHandlers { get; }

            public Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Pong() { Message = request.Message + " Pong" });
            }
        }

        public class Level0PingHandler : IRequestHandler<Ping, Pong>
        {
            public Level0PingHandler()
            {
                NextHandlers = new ConcurrentDictionary<Type, object>();
                NextHandlers.TryAdd(typeof(Level1PingHandler), new Level1PingHandler());
                NextHandlers.TryAdd(typeof(Level1AlternatePingHandler), new Level1AlternatePingHandler());
                NextHandlers.TryAdd(typeof(Level1AnotherAlternatePingHandler), new Level1AnotherAlternatePingHandler());
            }

            public ConcurrentDictionary<Type, object> NextHandlers { get; }

            public async Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
            {
                if (request.LevelType == PingLevelTypes.Level0)
                {
                    return await Task.FromResult(new Pong() { Message = request.Message + " Pong" + " 0" });
                }
                else if (request.LevelType <= PingLevelTypes.Level1Alternate)
                {
                    return await ((Level1AlternatePingHandler)NextHandlers[typeof(Level1AlternatePingHandler)]).Handle(request, cancellationToken);
                }
                else if (request.LevelType <= PingLevelTypes.Level1AnotherAlternate)
                {
                    return await ((Level1AnotherAlternatePingHandler)NextHandlers[typeof(Level1AnotherAlternatePingHandler)]).Handle(request, cancellationToken);
                }
                else
                {
                    return await ((Level1PingHandler)NextHandlers[typeof(Level1PingHandler)]).Handle(request, cancellationToken);
                }
            }
        }

        public class Level1AlternatePingHandler : IRequestHandler<Ping, Pong>
        {
            public ConcurrentDictionary<Type, object> NextHandlers { get; }

            public async Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
            {
                if (request.LevelType <= PingLevelTypes.Level1Alternate)
                {
                    return await Task.FromResult(new Pong() { Message = request.Message + " Pong" + " 1 Alternate" });
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public class Level1AnotherAlternatePingHandler : IRequestHandler<Ping, Pong>
        {
            public ConcurrentDictionary<Type, object> NextHandlers { get; }

            public async Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
            {
                if (request.LevelType <= PingLevelTypes.Level1AnotherAlternate)
                {
                    return await Task.FromResult(new Pong() { Message = request.Message + " Pong" + " 1 Another Alternate" });
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public class Level1PingHandler : IRequestHandler<Ping, Pong>
        {
            public Level1PingHandler()
            {
                NextHandlers = new ConcurrentDictionary<Type, object>();
                NextHandlers.TryAdd(typeof(Level2PingHandler), new Level2PingHandler());
            }

            public ConcurrentDictionary<Type, object> NextHandlers { get; }

            public async Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
            {
                if (request.LevelType <= PingLevelTypes.Level1)
                {
                    return await Task.FromResult(new Pong() { Message = request.Message + " Pong" + " 1" });
                }
                else
                {
                    return await ((Level2PingHandler)NextHandlers[typeof(Level2PingHandler)]).Handle(request, cancellationToken);
                }
            }
        }

        public class Level2PingHandler : IRequestHandler<Ping, Pong>
        {
            public ConcurrentDictionary<Type, object> NextHandlers { get; }

            public async Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
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

        [TestMethod]
        [TestCategory("Tree Send Tests")]
        public async Task Should_Resolve_Level0_Handler()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ServiceFactory>(sp => sp.GetService);

            services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(IChores), typeof(TreeSendTests))
            .AddClasses()
            .AsImplementedInterfaces());

            serviceProvider = services.BuildServiceProvider();
            var chores = serviceProvider.GetRequiredService<IChores>();
            var expected = "Ping Pong 0";

            // Act
            var response = await chores.SendToTree(new Ping { Message = "Ping", LevelType = PingLevelTypes.Level0 }, handlerType: typeof(Level0PingHandler));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Tree Send Tests")]
        public async Task Should_Resolve_Level1_Handler()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ServiceFactory>(sp => sp.GetService);

            services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(IChores), typeof(TreeSendTests))
            .AddClasses()
            .AsImplementedInterfaces());

            serviceProvider = services.BuildServiceProvider();
            var chores = serviceProvider.GetRequiredService<IChores>();
            var expected = "Ping Pong 1";

            // Act
            var response = await chores.SendToTree(new Ping { Message = "Ping", LevelType = PingLevelTypes.Level1 }, handlerType: typeof(Level0PingHandler));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Tree Send Tests")]
        public async Task Should_Resolve_Level1Alternate_Handler()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ServiceFactory>(sp => sp.GetService);

            services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(IChores), typeof(TreeSendTests))
            .AddClasses()
            .AsImplementedInterfaces());

            serviceProvider = services.BuildServiceProvider();
            var chores = serviceProvider.GetRequiredService<IChores>();
            var expected = "Ping Pong 1 Alternate";

            // Act
            var response = await chores.SendToTree(new Ping { Message = "Ping", LevelType = PingLevelTypes.Level1Alternate }, handlerType: typeof(Level0PingHandler));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Tree Send Tests")]
        public async Task Should_Resolve_Level1AnotherAlternate_Handler()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ServiceFactory>(sp => sp.GetService);

            services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(IChores), typeof(TreeSendTests))
            .AddClasses()
            .AsImplementedInterfaces());

            serviceProvider = services.BuildServiceProvider();
            var chores = serviceProvider.GetRequiredService<IChores>();
            var expected = "Ping Pong 1 Another Alternate";

            // Act
            var response = await chores.SendToTree(new Ping { Message = "Ping", LevelType = PingLevelTypes.Level1AnotherAlternate }, handlerType: typeof(Level0PingHandler));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Tree Send Tests")]
        public async Task Should_Resolve_Level2_Handler()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ServiceFactory>(sp => sp.GetService);

            services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(IChores), typeof(TreeSendTests))
            .AddClasses()
            .AsImplementedInterfaces());

            serviceProvider = services.BuildServiceProvider();
            var chores = serviceProvider.GetRequiredService<IChores>();
            var expected = "Ping Pong 2";

            // Act
            var response = await chores.SendToTree(new Ping { Message = "Ping", LevelType = PingLevelTypes.Level2 }, handlerType: typeof(Level0PingHandler));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Tree Send Tests")]
        public async Task Should_Resolve_Main_Handler()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ServiceFactory>(sp => sp.GetService);

            services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(IChores), typeof(TreeSendTests))
            .AddClasses()
            .AsImplementedInterfaces());

            serviceProvider = services.BuildServiceProvider();
            var chores = serviceProvider.GetRequiredService<IChores>();
            var expected = "Ping Pong";

            // Act
            var response = await chores.SendToTree(new Ping { Message = "Ping" }, typeof(PingHandler));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Tree Send Tests")]
        [ExpectedException(typeof(Exception))]
        public async Task Should_Throw_Exception()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ServiceFactory>(sp => sp.GetService);

            services.Scan(scanner => scanner
            .FromAssembliesOf(typeof(IChores), typeof(TreeSendTests))
            .AddClasses()
            .AsImplementedInterfaces());

            serviceProvider = services.BuildServiceProvider();
            var chores = serviceProvider.GetRequiredService<IChores>();

            // Act
            var response = await chores.SendToTree(new Ping { Message = "Ping", LevelType = PingLevelTypes.Level3 }, handlerType: typeof(Level0PingHandler));
        }
    }
}
