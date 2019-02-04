namespace Chores.Samples
{
    public class Ping : IRequest<Pong>
    {
        public string Message { get; set; }
    }
}