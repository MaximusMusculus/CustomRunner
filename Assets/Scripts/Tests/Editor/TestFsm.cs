using NUnit.Framework;
using StateMachine;
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
        var fsm = new SimpleFsm();
        
        fsm.RegisterState("1", new State("1"));
        fsm.RegisterState("2", new State("2"));
        fsm.RegisterState("3", new State("3"));
        
        var from_1_to_2 = new Condition();
        var from_2_to_3 = new Condition();
        var from_3_to_1 = new Condition();
        var from_3_to_2 = new Condition();
        
        fsm.RegisterTransition("1", "2", from_1_to_2);
        fsm.RegisterTransition("2", "3", from_2_to_3);
        fsm.RegisterTransition("3", "1", from_3_to_1);
        fsm.RegisterTransition("3", "2", from_3_to_2);
        fsm.LaunchState("1");
        
        fsm.Tick();
        fsm.Tick();
        fsm.Tick();
        
        from_1_to_2.SetSuccess(true);
        fsm.Tick();
        from_2_to_3.SetSuccess(true);
        fsm.Tick();
    }
    
    public class State : IState, ITickable
    {
        private string _state;

        public State(string state)
        {
            _state = state;
        }
        

        public void Tick()
        {
            Debug.Log(_state + " tick");
        }

        public void Enter()
        {
            Debug.Log(_state + " enter");
        }

        public void Exit()
        {
            Debug.Log(_state + " exit");
        }
    }
    
    public class Condition : ICondition
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