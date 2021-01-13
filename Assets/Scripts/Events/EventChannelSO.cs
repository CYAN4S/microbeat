using UnityEngine;
using UnityEngine.Events;

public abstract class EventChannelSO<T> : ScriptableObject
{
    public T value;
    public event UnityAction<T> onEventRaised;

    public void RaiseEvent(T data)
    {
        value = data;
        onEventRaised?.Invoke(data);
    }
}