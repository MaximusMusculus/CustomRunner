using Core;
using NUnit.Framework;
using UnityEngine;
using VContainer.Unity;


/// <summary>
/// тут должен быть тыст для фсм, но его нет ))
/// </summary>
public class TestFsm
{
    [Test]
    public void Test()
    {
        var fsm = new SimpleStatesBehaviour();
        
        fsm.AddState("1", new Behaviour("1"), true);
        fsm.AddState("2", new Behaviour("2"));
        fsm.AddState("3", new Behaviour("3"));
        
        var from_1_to_2 = new Condition();
        var from_2_to_3 = new Condition();
        var from_3_to_1 = new Condition();
        var from_3_to_2 = new Condition();
        
        fsm.AddTransition("1", "2", from_1_to_2.Check);
        fsm.AddTransition("2", "3", from_2_to_3.Check);
        fsm.AddTransition("3", "1", from_3_to_1.Check);
        fsm.AddTransition("3", "2", from_3_to_2.Check);
        fsm.Activate();
        
        fsm.Update(1);
        fsm.Update(1);
        fsm.Update(1);
        
        from_1_to_2.SetSuccess(true);
        fsm.Update(1);
        from_2_to_3.SetSuccess(true);
        fsm.Update(1);
    }
    
    public class Behaviour : IBehaviour, ITickable
    {
        private string _state;

        public Behaviour(string state)
        {
            _state = state;
        }
        

        public void Tick()
        {
            Debug.Log(_state + " tick");
        }

        public void Activate()
        {
            Debug.Log(_state + " enter");
        }

        public void Deactivate()
        {
            Debug.Log(_state + " exit");
        }

        public void Update(float deltaTime)
        {
            Debug.Log(_state + " Update");
        }
    }
    
    public class Condition 
    {
        private bool _success;
        
        public void SetSuccess(bool success)
        {
            _success = success;
        }
        
        public bool Check()
        {
            return _success;
        }
    }
}