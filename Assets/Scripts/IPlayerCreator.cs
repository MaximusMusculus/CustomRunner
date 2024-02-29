public interface IPlayerCreator
{
    void CreatePlayer();
}


public class SimplePlayer : IPlayerCreator
{
    public void CreatePlayer()
    {
        throw new System.NotImplementedException();
    }
}