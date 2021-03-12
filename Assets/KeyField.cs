using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class KeyField : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Image panel;
    
    public event UnityAction<KeyCode> OnValueChangeByInput;
    
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
                if (keycode == KeyCode.Escape) continue;
                if (keycode >= KeyCode.Mouse0) break;
                
                if (UnityEngine.Input.GetKeyDown(keycode))
                {
                    value = keycode;
                    text.text = keycode.ToString();
                    OnValueChangeByInput?.Invoke(keycode);
                    target = null;
                }
            }
        }
    }

    public void SetValue(KeyCode key)
    {
        value = key;
        text.text = key.ToString();
    }
}
