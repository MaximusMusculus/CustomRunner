using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Game.Controllers.Behaviour;
using Game.Shared;
using UnityEngine;


[System.Serializable]
public class CharacterConfig
{
    public string characterPrefabName;
    public List<PropertyData> properties;
}

public interface ICharacterFactory
{
    UniTask<ICharacterContainer> CreatePlayer(CharacterConfig config);
    SimpleFsm CreatePlayerBehaviour(ICharacterContainer character);
}

/// <summary>
/// Создает персонажа игрока из конфига.
/// Так же создает поведение игрока (возможно его нужно вынести отдельно)
/// </summary>
public class CharacterFactory : ICharacterFactory
{
    private readonly IInput _input;
    private readonly LayersConfig _layersConfig;
    
    private const string StateRun = "Run";
    private const string StateJump = "Jump";
    private const string StateFall = "Fall";
    public const string StateDead = "Dead";

    public CharacterFactory(IInput input, LayersConfig layersConfig)
    {
        _input = input;
        _layersConfig = layersConfig;
    }

    public async UniTask<ICharacterContainer> CreatePlayer(CharacterConfig config)
    {
        var characterPrefab = LoadPrefab(config.characterPrefabName);
        var character = Spawn(characterPrefab);
        var view = character.GetComponent<CharacterView>();
        /*var baseValues = new Properties();
        foreach (var info in config.properties)
        {
            baseValues.Set(info.property, info.value);
        }*/

        await UniTask.Yield();
        var characterContainer = new CharacterContainer(view.Rigidbody, view.Animator);
        return characterContainer;
    }

    public SimpleFsm CreatePlayerBehaviour(ICharacterContainer character) //+? behaviourConfg
    {
        var groundChecker = new OverlapCircleChecker(character, _layersConfig.ground, 0.85f);
        var enemyHitChecker = new OverlapCircleChecker(character, _layersConfig.enemy, 0.6f);

        var fsm = new SimpleFsm();
        var jumpState = new CharacterJump(character, _input);

        fsm.AddState(StateRun, new CharacterRun(character, _input));
        fsm.AddState(StateJump, jumpState);
        fsm.AddState(StateFall, new CharacterFall(character, _input));
        fsm.AddState(StateDead, new CharacterDead(character));

        fsm.AddTransition(StateRun, StateJump, _input.GetIsJump);
        fsm.AddTransition(StateRun, StateFall, () => !groundChecker.Check());

        fsm.AddTransition(StateJump, StateFall, jumpState.IsJumpEnd);
        fsm.AddTransition(StateFall, StateRun, groundChecker.Check);

        //добавить any state
        fsm.AddTransition(StateRun, StateDead, () => enemyHitChecker.Check());
        fsm.AddTransition(StateJump, StateDead, () => enemyHitChecker.Check());
        fsm.AddTransition(StateFall, StateDead, () => enemyHitChecker.Check());

        fsm.LaunchState(StateRun);

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