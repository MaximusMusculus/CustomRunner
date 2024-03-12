using UnityEngine;

public class OverlapCircleChecker 
{
    private readonly LayerMask _groundLayer;
    private readonly ICharacterContainer _character;
    private readonly Collider2D[] results = new Collider2D[6];
    private readonly float _radius;

    public OverlapCircleChecker(ICharacterContainer character, LayerMask groundLayer, float radius)
    {
        _character = character;
        _groundLayer = groundLayer;
        _radius = radius;
    }

    public bool Check()
    {
        var count = Physics2D.OverlapCircleNonAlloc(_character.MoveComponent.Position, _radius, results, _groundLayer);
        if (count > 0)
        {
            return true;
        }
        return false;
    }
}