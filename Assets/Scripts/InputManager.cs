using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public static readonly KeyCode[] playKeyCodes = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };
    public static readonly KeyCode[] speedKeyCodes = { KeyCode.T, KeyCode.Y };
    public static Action<int> OnPlayKey = _ => {}, OnPlayKeyDown = _ => {}, OnPlayKeyUp = _ => {};
    public static Action<int> OnSpeedKeyDown = _ => {};

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        for (int i = 0; i < playKeyCodes.Length; i++)
        {
            KeyCode key = playKeyCodes[i];

            if (Input.GetKey(key))
                OnPlayKey(i);

            if (Input.GetKeyDown(key))
                OnPlayKeyDown(i);

            if (Input.GetKeyUp(key))
                OnPlayKeyUp(i);
        }

        for (int i = 0; i < speedKeyCodes.Length; i++)
        {
            KeyCode key = speedKeyCodes[i];
            if (Input.GetKeyDown(key))
                OnSpeedKeyDown(i);
        }
    }
}
