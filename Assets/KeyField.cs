using System;
using System.Collections.Generic;
using System.Linq;
using FileIO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class KeyField : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Image panel;

    public event UnityAction<Key> OnValueChangeByInput;

    private Button button;
    private Key value;

    public BindingEnum targetEnum;
    public int targetIndex;

    public Key Value
    {
        get => value;
        set
        {
            this.value = value;
            text.text = value.ToString();
        }
    }

    public static readonly IEnumerable<Key> Keys = Enum.GetValues(typeof(Key)).Cast<Key>();
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

        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            foreach (Key key in Keys)
            {
                if (key == Key.Escape || key == Key.None) continue;
                if (key >= Key.F1) break;

                if (Keyboard.current[key].wasPressedThisFrame)
                {
                    OnValueChangeByInput?.Invoke(key);
                    target = null;
                }
            }
        }
    }

    public void SetValue(Key key)
    {
        Value = key;
        text.text = key.ToString();
    }

    public void RemoveValue()
    {
        text.text = "-";
    }

    public void SetInteractable(bool value)
    {
        button.interactable = value;
    }
}