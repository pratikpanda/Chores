using Chores.Tree;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chores.Samples.Tree
{
    public class PingHandler : IRequestHandler<Ping, Pong>
    {
        public ConcurrentDictionary<Type, object> NextHandlers { get; }

        public Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Pong { Message = "pong" });
        }
    }
}
