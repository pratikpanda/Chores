using Chores.Chain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chores.Tests
{
    [TestClass]
    public class ChainRequestHandlerTests
    {
        public enum PingLevelTypes
        {
            Level0,
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
            public override IRequestHandler<Ping, Pong> NextHandler { get; }

            public override Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Pong() { Message = request.Message + " Pong" });
            }
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

        [TestMethod]
        [TestCategory("Chain Request Handler Tests")]
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
        [TestCategory("Chain Request Handler Tests")]
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
        [TestCategory("Chain Request Handler Tests")]
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
        [TestCategory("Chain Request Handler Tests")]
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
        [TestCategory("Chain Request Handler Tests")]
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