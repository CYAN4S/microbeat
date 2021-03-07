using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class KeyField : MonoBehaviour
{
    private Button button;
    public static KeyField target = null;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnClick()
    {
        target = this;
    }

    private void Update()
    {
        if (target != this)
        {
            return;
        }
        
        
    }
}
