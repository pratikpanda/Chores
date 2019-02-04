namespace Chores
{
    /// <summary>
    /// Allows for generic type constraints of objects implementing IRequest.
    /// </summary>
    public interface IBaseRequest { }

    /// <summary>
    /// Marker interface to represent a request with a response.
    /// </summary>
    /// <typeparam name="TResponse">Response type.</typeparam>
    public interface IRequest<out TResponse> { }
}