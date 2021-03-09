using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class KeyField : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Image panel; 
    private Button button;
    private KeyCode value;
    
    public static readonly Array Keycodes = Enum.GetValues(typeof(KeyCode));
    public static KeyField target = null;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnClick()
    {
        target = this;
    }

    private void Update()
    {
        if (target != this)
        {
            return;
        }

        if (UnityEngine.Input.anyKeyDown)
        {
            foreach (KeyCode keycode in Keycodes)
            {
                if (UnityEngine.Input.GetKeyDown(keycode))
                {
                    value = keycode;
                    text.text = keycode.ToString();
                    target = null;
                }
            }
        }
    }

    public void ChangeValue(KeyCode key)
    {
        value = key;
        text.text = key.ToString();
    }
}
