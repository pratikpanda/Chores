using Chores.Chain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chores.Samples.Chain
{
    public class PingHandler : IRequestHandler<Ping, Pong>
    {
        public IRequestHandler<Ping, Pong> NextHandler { get; }

        public Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Pong { Message = "pong" });
        }
    }
}
