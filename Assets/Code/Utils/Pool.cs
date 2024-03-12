using System;
using System.Collections.Generic;

namespace Core
{
    public interface IResettable
    {
        void Reset();
    }

    public interface IPool<T> where T : IResettable
    {
        T Get();
        void Release(T element);
    }



    public class StackPool<T> : IPool<T> where T : IResettable
    {
        private readonly Func<T> _createDelegate;
        private readonly Stack<T> _pooledElems;
        private const int DefaultCapacity = 10;

        public StackPool(Func<T> createDelegate, int prePoledCount = 0)
        {
            _createDelegate = createDelegate;
            var startCapacity = prePoledCount > DefaultCapacity ? prePoledCount : DefaultCapacity;
            _pooledElems = new Stack<T>(startCapacity);

            for (var i = 0; i < prePoledCount; i++)
            {
                _pooledElems.Push(_createDelegate());
            }
        }

        public T Get()
        {
            if (_pooledElems.Count > 0)
            {
                return _pooledElems.Pop();
            }

            return _createDelegate();
        }

        public void Release(T element)
        {
            element.Reset();
            _pooledElems.Push(element);
        }
    }
}