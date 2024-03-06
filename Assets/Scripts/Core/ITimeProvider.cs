namespace Core
{
    /// <summary>
    /// Представляет интерфейс для получения времени
    /// </summary>
    public interface ITimeProvider
    {
        float DeltaTime { get; }
        //Time
        //FixedDeltaTime
        //UnscaledDeltaTime
        //...
    }
}