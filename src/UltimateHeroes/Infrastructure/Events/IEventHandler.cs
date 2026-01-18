namespace UltimateHeroes.Infrastructure.Events
{
    /// <summary>
    /// Interface für Event Handler
    /// </summary>
    public interface IEventHandler<T> where T : IGameEvent
    {
        void Handle(T eventData);
    }
}
