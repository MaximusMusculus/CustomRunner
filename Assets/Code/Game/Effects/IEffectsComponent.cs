using System;
using System.Collections.Generic;
using System.Linq;
using Game.Properties;
using Game.Shared;

namespace Game.Components
{
    public abstract class BaseEffect
    {
        public bool IsDone { get; }
        
        //можно ли применить эффект. По идее толко сам эффект знает, свои условия применения
        public abstract bool CanApply(IComponentsEntity entity);
        public abstract void Apply(IComponentsEntity target);
        public abstract void Cancel();
        public virtual void Tick(float deltaTime) { }
        
        
        public abstract class EffectShoot 
        {
           public Id Id { get; }
            protected EffectShoot(Id id)
            {
                Id = id;
            }

            public abstract void Recovery(BaseEffect baseEffect);
            
        }
        public abstract EffectShoot GetShoot();
        
    }

    public class EffectConfig  : IDefinition
    {
        public Id Id { get; }
    }


    
    public class PropertyModifyConfig 
    {
        public TypeProperty typeProperty;
        public float modifyValue;
        public float modifyMultiplier;
    } 
    
    
    public class PropertyEffectConfig : EffectConfig
    {
        public float duration;
        public PropertyModifyConfig config;
    }
    
    public class PropertyBaseEffect : BaseEffect
    {
        private readonly float _duration;
        private readonly BaseProperty _speedModifier;
        private float _elapsedTime;
        private TypeProperty _typeProperty;
        
        public bool IsDone => _elapsedTime >= _duration;
        private IPropertyComponent _propertyComponent;

        private PropertyEffectConfig _config;

        public PropertyBaseEffect(PropertyEffectConfig config)
        {
            _config = config;
            _duration = config.duration;
            _typeProperty = config.config.typeProperty;
            _speedModifier = new BaseProperty(config.config.modifyValue, config.config.modifyMultiplier);
        }

        public override bool CanApply(IComponentsEntity target) 
        {
            return target.TryGetComponent<IPropertyComponent>(out var component);
        }
        
        public override void Apply(IComponentsEntity target)
        {
            _elapsedTime = 0;
            target.TryGetComponent<IPropertyComponent>(out var component);
            component.AddModifier(_typeProperty, _speedModifier);
        }

        public override void Cancel()
        {
            _propertyComponent.RemoveModifier(TypeProperty.Speed, _speedModifier);
        }

        public override void Tick(float deltaTime)
        {
            _elapsedTime += deltaTime;
        }

        public override EffectShoot GetShoot()
        {
            return new PropertyEffectShoot(_config);
        }


        public class PropertyEffectShoot: EffectShoot
        {
            public PropertyEffectShoot(PropertyEffectConfig config) : base(config.Id)
            {
            }

            public override void Recovery(BaseEffect baseEffect)
            {
                var concreteEffect = baseEffect as PropertyBaseEffect;
                //восстановить все поля эффекта (счетчики, значения и тд)
            }
        }
    }
    
    
    public interface IEffectsComponent : IComponent
    {
        void ApplyEffect(TypeEffect typeEffect);
    }

    public enum TypeEffect
    {
        PropertyEffect,
    }

    public interface IEffectFactory
    {
        BaseEffect Create(EffectConfig config);
    }

    public class EffectFactory : IEffectFactory
    {
        public BaseEffect Create(EffectConfig config)
        {
            if (config is PropertyEffectConfig effectConfig)
            {
                return new PropertyBaseEffect(effectConfig);
            }
            throw new ArgumentException("Can't create effect from config " + config.GetType() );
        }
    }
    


    //компонент с эффектами  (я знаю своего хозяина, а эффекты я храню те, что на меня накладываются)
    public class Effects : IEffectsComponent
    {
        private List<BaseEffect> _effects = new List<BaseEffect>();
        private IComponentsEntity _target;
        private IEffectFactory _factory;
        
        public void ApplyEffect(TypeEffect effect)
        {
            //тут диспатч события о эффекте
        }
        
        public EffectsSnapshot GetSnapShot()
        {
            var effectShoots = new List<BaseEffect.EffectShoot>(_effects.Count);
            effectShoots.AddRange(_effects.Select(effect => effect.GetShoot()));
            return new EffectsSnapshot(effectShoots);
        }

        public interface IConfigs
        {
            public EffectConfig Get(Id id);
            public Id Get(EffectConfig config);
        }
  

        //не важен способ сериализации
        //Use configs serialize
        public class EffectsSnapshot 
        {
            private List<BaseEffect.EffectShoot> _effects;
            
            public EffectsSnapshot(List<BaseEffect.EffectShoot> effects)
            {
                _effects = effects;
            }

            public void Restore(Effects target, IConfigs configs) //component + config?
            {
                //target.ClearAllEffects();
                
                //1. эффекты хранят ВСЕ поля в т.ч и конфиги
                //2. эффекты хранят id конфига
                foreach (var shoot in _effects)
                {
                    var cnf = configs.Get(shoot.Id);
                    var effect = target._factory.Create(cnf);
                    //непонятна последователньость восстановить и применить или наоборот.
                    effect.Apply(target._target);
                    shoot.Recovery(effect);
                }
            }
        }

    }
    
    

}