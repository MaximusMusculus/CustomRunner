using System.Collections.Generic;
using NUnit.Framework;

namespace TestEffects
{
    public class TestCustom
    {
        [Test]
        public void TestLink()
        {
            var characterService = new CharacterService();
            var effectService = new EffectService();
            
            characterService.AddCharacter(1, new Character(1, "1", effectService.Create()));
            characterService.AddCharacter(2, new Character(2, "2", effectService.Create()));
            
            var effect = new TestEffect(characterService, 1, 2);
            characterService.GetCharacter(2).AddEffect(effect);
            characterService.Update();
            
            characterService.RemoveCharacter(1);
            characterService.Update();
        }

        [Test]
        public void TestLink2()
        {
            var character1 = new Character(1, "1", new Effects());
            var character2 = new Character(2, "2", new Effects());
            character2.AddEffect(new TestRefEffects(character1, character2));
            
            character2.Update();
            //character1.IsAlive = false;
            character2.Update();




        }
        
        
        
        public class TestRefEffects : IEffect
        {
            private  Character _character1;
            private  Character _character2;

            public TestRefEffects(Character character1, Character character2)
            {
                _character1 = character1;
                _character2 = character2;
            }

            public void Update()
            {
                if(_character1.IsAlive) _character1.Log();
                if(_character2.IsAlive) _character2.Log();
            }
        }

       
        public interface IComponent { }
        public interface ICharacterService
        {
            Character GetCharacter(int id);
        }
        
        public class CharacterService : ICharacterService
        {
            private Dictionary<int, Character> _characters = new Dictionary<int, Character>();
            public Character GetCharacter(int id)
            {
                return _characters[id];
            }
            public void AddCharacter(int id, Character character)
            {
                _characters.Add(id, character);
            }
            public void RemoveCharacter(int id)
            {
                _characters.Remove(id);
            }

            public void Update()
            {
                foreach (var character in _characters)
                {
                    character.Value.Update();
                }
            }
                
        }
        
        
        public interface IEffectService
        {
            
        }
        public class EffectService : IEffectService
        {
            private Dictionary<int, Effects> _effects = new Dictionary<int, Effects>(); // effectComponent

            public Effects Create()
            {
                return new Effects();
            }
            
            public void AddEffect(TestEffect effect, int ownerId)
            {
                if (!_effects.ContainsKey(ownerId))
                {
                    _effects.Add(ownerId, new Effects());
                }
                _effects[ownerId].AddEffect(effect);
            }
            
            public Effects GetEffects(int ownerId)
            {
                return _effects[ownerId];
            }
        }
        
        
        public interface IEffect
        {
            void Update();
        }
        
        public class Effects : IComponent
        {
            public int Id { get; private set; }
            public List<IEffect> _effects = new List<IEffect>();
            
            public void AddEffect(IEffect effect)
            {
                _effects.Add(effect);
            }
            public void Update()
            {
                foreach (var effect in _effects)
                {
                    effect.Update();
                }
            }
        }
        
        
        public class Character
        {
            public int Id { get; private set; }
            private string _name;
            private Effects _effects;

            public bool IsAlive { get; set; } = true;

            public Character(int id, string name, Effects effects)
            {
                Id = id;
                _name = name;
                _effects = effects;
            }

            public void AddEffect(IEffect effect)
            {
                _effects.AddEffect(effect);
            }
            
            public void Update()
            {
                _effects.Update();
            }

            public void Log()
            {
                UnityEngine.Debug.Log("Character " + _name + " is updated");
            }
        }
        public class TestEffect : IEffect
        {
            private ICharacterService _characterService;
            private int _source;
            private int _target;

            public TestEffect(ICharacterService characterService, int source, int target)
            {
                _characterService = characterService;
                _source = source;
                _target = target;
            }
            
            public void Update()
            {
                _characterService.GetCharacter(_source).Log();
                _characterService.GetCharacter(_target).Log();
            }
        }
        
    }
}