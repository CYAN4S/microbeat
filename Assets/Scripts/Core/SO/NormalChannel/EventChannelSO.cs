using UnityEngine;
using UnityEngine.Events;

namespace Core.SO.NormalChannel
{
    public abstract class EventChannelSO<T> : ScriptableObject
    {
        public T value;
        public event UnityAction<T> OnEventRaised;

        public void RaiseEvent(T data)
        {
            value = data;
            OnEventRaised?.Invoke(data);
        }
    }
}