namespace Core
{
    /// <summary>
    /// Интерфейс для обновления объекта playerloop 
    /// </summary>
    public interface IUpdate
    {
        void Update(float deltaTime);
    }

    public interface IFixedUpdate
    {
        void FixedUpdate();
    }
}