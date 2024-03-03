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
    
    /// <summary>
    /// Интерфейс для обновления объекта playerloop 
    /// </summary>
    public interface IUpdate
    {
        void Update(float deltaTime);
    }
}