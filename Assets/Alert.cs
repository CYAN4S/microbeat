using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{
    [SerializeField] private Text text;

    private void Awake()
    {
        Destroy(this);
    }

    public void SetMessage(string message)
    {
        text.text = message;
    }
}
