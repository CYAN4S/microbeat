using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VoidEventListener : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO channel = default;

    public UnityEvent onEventRaised;

    private void OnEnable()
    {
        if (channel != null)
            channel.onEventRaised += Respond;
    }

    private void OnDisable()
    {
        if (channel != null)
            channel.onEventRaised -= Respond;
    }

    private void Respond()
    {
        onEventRaised?.Invoke();
    }
}
