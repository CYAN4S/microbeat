using System;
using System.Collections;
using System.Collections.Generic;
using SO.NormalChannel;
using UnityEngine;

public class AlertManager : MonoBehaviour
{
    [SerializeField] private Alert prefab;
    [SerializeField] private StringEventChannelSO channel;
    [SerializeField] private Canvas canvas;
    private void OnEnable()
    {
        channel.OnEventRaised += ShowAlert;
    }

    private void OnDisable()
    {
        channel.OnEventRaised -= ShowAlert;
    }

    public void ShowAlert(string message)
    {
        var alert = Instantiate(prefab, canvas.transform);
        alert.SetMessage(message);
    }
}
