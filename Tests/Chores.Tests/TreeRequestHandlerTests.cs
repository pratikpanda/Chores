using Chores.Tree;
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
    public class TreeRequestHandlerTests
    {
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

        public class PingHandler : RequestHandler<Ping, Pong>
        {
            public override ConcurrentDictionary<Type, object> NextHandlers { get; }

            public override Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Pong() { Message = request.Message + " Pong" });
            }
        }

        public class Level0PingHandler : RequestHandler<Ping, Pong>
        {
            public Level0PingHandler()
            {
                NextHandlers = new ConcurrentDictionary<Type, object>();
                NextHandlers.TryAdd(typeof(Level1PingHandler), new Level1PingHandler());
                NextHandlers.TryAdd(typeof(Level1AlternatePingHandler), new Level1AlternatePingHandler());
                NextHandlers.TryAdd(typeof(Level1AnotherAlternatePingHandler), new Level1AnotherAlternatePingHandler());
            }

            public override ConcurrentDictionary<Type, object> NextHandlers { get; }

            public override async Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
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

        public class Level1AlternatePingHandler : RequestHandler<Ping, Pong>
        {
            public override ConcurrentDictionary<Type, object> NextHandlers { get; }

            public override async Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
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

        public class Level1AnotherAlternatePingHandler : RequestHandler<Ping, Pong>
        {
            public override ConcurrentDictionary<Type, object> NextHandlers { get; }

            public override async Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
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

        public class Level1PingHandler : RequestHandler<Ping, Pong>
        {
            public Level1PingHandler()
            {
                NextHandlers = new ConcurrentDictionary<Type, object>();
                NextHandlers.TryAdd(typeof(Level2PingHandler), new Level2PingHandler());
            }

            public override ConcurrentDictionary<Type, object> NextHandlers { get; }

            public override async Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
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

        public class Level2PingHandler : RequestHandler<Ping, Pong>
        {
            public override ConcurrentDictionary<Type, object> NextHandlers { get; }

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

        [TestMethod]
        [TestCategory("Tree Request Handler Tests")]
        public async Task Should_Call_Abstract_Handler()
        {
            // Arrange
            IRequestHandler<Ping, Pong> requestHandler = new PingHandler();
            var expected = "Ping Pong";

            // Act
            var response = await requestHandler.Handle(new Ping() { Message = "Ping" }, default(CancellationToken));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Tree Request Handler Tests")]
        public async Task Should_Call_First_Abstract_Handler()
        {
            // Arrange
            IRequestHandler<Ping, Pong> requestHandler = new Level0PingHandler();
            var expected = "Ping Pong 0";

            // Act
            var response = await requestHandler.Handle(new Ping() { Message = "Ping", LevelType = PingLevelTypes.Level0 }, default(CancellationToken));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Tree Request Handler Tests")]
        public async Task Should_Call_Second_Alternate_Abstract_Handler()
        {
            // Arrange
            IRequestHandler<Ping, Pong> requestHandler = new Level0PingHandler();
            var expected = "Ping Pong 1 Alternate";

            // Act
            var response = await requestHandler.Handle(new Ping() { Message = "Ping", LevelType = PingLevelTypes.Level1Alternate }, default(CancellationToken));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Tree Request Handler Tests")]
        public async Task Should_Call_Second_Another_Alternate_Abstract_Handler()
        {
            // Arrange
            IRequestHandler<Ping, Pong> requestHandler = new Level0PingHandler();
            var expected = "Ping Pong 1 Another Alternate";

            // Act
            var response = await requestHandler.Handle(new Ping() { Message = "Ping", LevelType = PingLevelTypes.Level1AnotherAlternate }, default(CancellationToken));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Tree Request Handler Tests")]
        public async Task Should_Call_Second_Abstract_Handler()
        {
            // Arrange
            IRequestHandler<Ping, Pong> requestHandler = new Level0PingHandler();
            var expected = "Ping Pong 1";

            // Act
            var response = await requestHandler.Handle(new Ping() { Message = "Ping", LevelType = PingLevelTypes.Level1 }, default(CancellationToken));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Tree Request Handler Tests")]
        public async Task Should_Call_Third_Abstract_Handler()
        {
            // Arrange
            IRequestHandler<Ping, Pong> requestHandler = new Level0PingHandler();
            var expected = "Ping Pong 2";

            // Act
            var response = await requestHandler.Handle(new Ping() { Message = "Ping", LevelType = PingLevelTypes.Level2 }, default(CancellationToken));

            // Assert
            Assert.AreEqual(expected, response.Message);
        }

        [TestMethod]
        [TestCategory("Tree Request Handler Tests")]
        [ExpectedException(typeof(Exception))]
        public async Task Should_Throw_Exception()
        {
            // Arrange
            IRequestHandler<Ping, Pong> requestHandler = new Level0PingHandler();

            // Act
            var response = await requestHandler.Handle(new Ping() { Message = "Ping", LevelType = PingLevelTypes.Level3 }, default(CancellationToken));
        }
    }
}
