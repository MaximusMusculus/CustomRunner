using Core;

namespace Game.Conditions
{
    public interface ICondition
    {
        bool Check();
    }
    
    public abstract class CheckCondition : ICondition, IUpdate
    {
        public abstract bool Check();
        public virtual void Update(float deltaTime)
        {
        }
        
        public virtual void Reset(){}
    }
}
