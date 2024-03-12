
using System;

namespace Game.Components
{
    //base interfaces
    //базовый скелет сущности и компонента
    public struct Id : IComparable
    {
        public uint Value => _id;
        private readonly uint _id;
        
        public Id(uint id)
        {
            _id = id;
        }
        public bool Equals(Id other)
        {
            return _id == other._id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Id && Equals((Id) obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public static bool operator ==(Id id1, Id id2)
        {
            return id1._id == id2._id;
        }

        public static bool operator !=(Id id1, Id id2)
        {
            return id1._id != id2._id;
        }
        
        public static Id Default { get; } = new Id(0);

        public override string ToString()
        {
            return _id.ToString();
        }

        public int CompareTo(object obj)
        {
            var id = (Id) obj;
            return _id.CompareTo(id._id);
        }
    }
    public interface IDefinition
    {
        Id Id { get; }
        //type? or group?
    }


    public interface IEntity
    {
    }

    public interface IEntities
    {
        bool TryGetEntity(Id entityId, out IEntity entity);
        bool TryGetEntityComponent<T>(Id entityId, out T entity) where T : IComponent;
    }


    public interface IComponent
    {
    }
    
    public interface IComponentsEntity : IEntity
    {
        bool TryGetComponent<T>(out T component) where T : IComponent;
    }
    
    
    
}