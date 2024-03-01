using UnityEngine;

public interface IPlayerSpawnPoint
{
    Vector3 GetPlayerSpawnPoint();
}

public class SimplePlayerSpawnPoint : IPlayerSpawnPoint
{
    private readonly Transform _singleSpawnPoint;

    public SimplePlayerSpawnPoint(Transform singleSpawnPoint)
    {
        _singleSpawnPoint = singleSpawnPoint;
    }

    public Vector3 GetPlayerSpawnPoint()
    {
        return _singleSpawnPoint.position;
    }
}