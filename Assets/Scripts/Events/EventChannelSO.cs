using UnityEngine;
using UnityEngine.Events;

public abstract class EventChannelSO<T> : ScriptableObject
{
    public event UnityAction<T> onEventRaised;

    public void RaiseEvent(T value)
    {
        onEventRaised?.Invoke(value);
    }
}