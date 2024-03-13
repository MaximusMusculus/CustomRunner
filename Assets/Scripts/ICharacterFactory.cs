using Core;
using Game.Components;
using Game.Controllers.Behaviour;
using Game.Properties;
using Game.Shared;
using UnityEngine;


public enum TypeMoveBehaviours
{
    // бегающая прыгающая лиса (может взять полет - в полете летит ка птица, но чуть ровнее).
    // лягуха, передвижение - прыжками (в пока держишь тап - идет прыжок вверх и в длину, при отпускании - вниз) /может останавливаться. пока на земле.
    // слизень - ползет - при прыжке становится неуязвимым - типа перетекает вперед. При подборе полета?
    // птица - перемещение как флапи берд тапами, если упасть - разобъется. Чем выше находится ,тем менее эффективнее тапы (если берет полет - летит прямо без тапов, тапы - меняют высоту аки прыжки)
}

[System.Serializable]
public class CharacterConfig
{
    public string prefabName;
    public string viewPrefabName;
    
    public PropertyConfig propertyConfig;
    //behaviourType
    //start items
    //start effects
    //start abitilies
}


public interface ICharacterFactory
{
    ComponentsContainer CreatePlayer(CharacterConfig config);
    SimpleStatesBehaviour CreatePlayerBehaviour(IComponentsEntity character);
}

public interface IComponentsFactory
{
    IPropertyComponent CreatePropertyComponent(PropertyConfig config);
    IMoveComponent CreateMoveComponent(Rigidbody2D rigidbody2D);
}

public class ComponentsFactory : IComponentsFactory
{
    public IMoveComponent CreateMoveComponent(Rigidbody2D rigidbody2D) //Type?
    {
        return new MoveComponent(rigidbody2D);
    }
    
    public IPropertyComponent CreatePropertyComponent(PropertyConfig config)
    {
        var property = new PropertyComponent();
        foreach (var info in config.properties)
        {
            property.AddProperty(info.property, info.value);
        }
        return property;
    }
}



/// <summary>
/// Создает персонажа игрока из конфига.
/// Так же создает поведение игрока (возможно его нужно вынести отдельно)
/// </summary>
public class CharacterFactory : ICharacterFactory
{
    private readonly IInput _input;

    
    private const string StateRun = "Run";
    private const string StateJump = "Jump";
    private const string StateFall = "Fall";

    private readonly IComponentsFactory _componentsFactory;

    public CharacterFactory(IInput input, IComponentsFactory componentsFactory)
    {
        _input = input;
        _componentsFactory = componentsFactory;
    }
    //утилизация персонажей 
    
    public ComponentsContainer CreatePlayer(CharacterConfig config) //+input?  - createCharacter?
    {
        var characterPrefab = LoadPrefab(config.prefabName);
        var character = Spawn(characterPrefab);
        var view = character.GetComponent<CharacterView>();
        
        //Конструирование
        var builder = ComponentsContainer.Builder(_componentsFactory);
        builder.NewEntity();
        builder.AddProperty(config.propertyConfig);
        builder.AddMove(view.Rigidbody);
        builder.AddBehaviourComponent();
        builder.AddAnimatorComponent(view.Animator);
        return builder.GetEntity();


        /*var property = _componentsFactory.CreatePropertyComponent(config.propertyConfig);
        var characterContainer = new CharacterContainer(view.Rigidbody, view.Animator, property);
        return characterContainer;*/
    }

    public SimpleStatesBehaviour CreatePlayerBehaviour(IComponentsEntity character) //+? behaviourConfig + type?
    {
        character.TryGetComponent<IMoveComponent>(out var moveComponent);
        character.TryGetComponent<IPropertyComponent>(out var propertyComponent);

        var fsm = new SimpleStatesBehaviour();
        var jumpState = new CharacterJump(moveComponent, propertyComponent, _input);

        fsm.AddState(StateRun, new CharacterRun(moveComponent, propertyComponent, _input), true);
        fsm.AddState(StateJump, jumpState);
        fsm.AddState(StateFall, new CharacterFall(_input, moveComponent, propertyComponent));
        
        fsm.AddTransition(StateRun, StateJump, _input.GetIsJump);
        fsm.AddTransition(StateRun, StateFall, () => !moveComponent.IsOnGround);
        fsm.AddTransition(StateJump, StateFall, jumpState.IsJumpEnd);
        fsm.AddTransition(StateFall, StateRun, () => moveComponent.IsOnGround);
        fsm.Activate();
        return fsm;
    }
    
    

    /// Загрузку префаба нужно делегировать отдельной системме
    private GameObject LoadPrefab(string prefabName)
    {
        return Resources.Load<GameObject>(prefabName);
    }

    private GameObject Spawn(GameObject prefab)
    {
        return GameObject.Instantiate(prefab);
    }
}