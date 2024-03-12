using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Game.Components
{
    /// <summary>
    /// Компонент, отвечающий за перемещение объекта
    /// аггрегирует воздействия и корректно их применяет по единому правилу  
    /// </summary>
    public class MoveComponent : IMoveComponent, IUpdate, IFixedUpdate
    {
        private const int StartMoveImpactsCount = 10;
        private const string GroundLayer = "Ground";
        
        private readonly Rigidbody2D _targetBody;
        private readonly List<Vector2> _moveImpacts = new(StartMoveImpactsCount);
        
        private Vector2 _frameVelocity;
        public Vector2 Velocity => _frameVelocity;
        
        //должно ли это быть здесь?
        public Transform Transform => _targetBody.transform;
        public bool IsOnGround { get; private set; }
        private readonly Collider2D[] _contacts = new Collider2D[1];
        private readonly ContactFilter2D _contactFilter;

        public MoveComponent(Rigidbody2D targetBody)
        {
            _targetBody = targetBody;
            _contactFilter = new ContactFilter2D();
            _contactFilter.SetLayerMask(LayerMask.GetMask(GroundLayer));
        }
        
        public Vector2 Position 
        {
            get => _targetBody.position;
            set => _targetBody.position = value;
        }

        public void Move(Vector2 impact)
        {
            _moveImpacts.Add(impact);
        }

        public void Update(float deltaTime)
        {
            IsOnGround = _targetBody.GetContacts(_contactFilter, _contacts) > 0;
            _frameVelocity = Vector2.zero;
            foreach (var impact in _moveImpacts)
            {
                _frameVelocity += impact;
            }
            _moveImpacts.Clear();
        }

        public void FixedUpdate()
        {
            _targetBody.MovePosition(_targetBody.position + _frameVelocity * Time.fixedDeltaTime);
        }
    }
}