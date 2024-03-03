using System;
using Core;
using Game.Conditions;
using TestConditions;

namespace TestGlobalEvents
{
    
    public interface IGlobalEventReceiver
    {
        string EventName { get; } 
        //type
        void OnReceive();//Context, ....
    }

    public class GlobalEvent
    {
        public string name;

        public GlobalEvent(string name)
        {
            this.name = name;
        }
    }
    public class ConcreteGlobalEvent<T> : GlobalEvent
    {
        public T EventData { get; }
        public ConcreteGlobalEvent(string name, T eventData) : base(name)
        {
            EventData = eventData;
        }
    }
    
    /*public class OnPlayerDeathEvent : ConcreteGlobalEvent<i>
    {
        public OnPlayerDeathEvent(string name, int eventData) : base(name, eventData)
        {
        }
    }*/
    
    
    
    public class ConditionGlobalEventCondition : ICondition, IGlobalEventReceiver
    {
        private readonly string _eventName;
        private bool _checkResult;
        
        public ConditionGlobalEventCondition(string eventName)
        {
            _eventName = eventName;
        }
        
        public void Reset()
        {
            _checkResult = false;
        }
        
        public bool Check()
        {
            return _checkResult;
        }

        public void Update()
        {
        }

        public string EventName { get; }

        public void OnReceive()
        {
            throw new NotImplementedException();
        }
    }
    
    public class ConditionInventoryCondition : ICondition
    {
        //private Inventory _inventory;
        private readonly TypeCompare _typeCompare;
        private readonly int _itemId; //orItemType
        private readonly float _count;
        
        private bool _checkResult;
        
        /*public CheckInventoryTrigger(ItemType itemType, int count)
        {
            _itemType = itemType;
            _count = count;
        }

        public void SetTarget(Inventory inventory)
        {
            _inventory = inventory;
        }*/
        
        public void Reset()
        {
            _checkResult = false;
        }
        
        public bool Check()
        {
            return _checkResult;
        }

        public void Update()
        {
        }
    }
}