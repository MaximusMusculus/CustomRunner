using UnityEngine;

namespace Game.Components
{
    public interface IMoveComponent : IComponent
    {
        Transform Transform { get; }
        
        bool IsOnGround { get; }
        Vector2 Velocity { get; }//влево, вправо, вверх, вниз, по прямой
        void Move(Vector2 impact);
        
        Vector2 Position { get; set; }
    }
}