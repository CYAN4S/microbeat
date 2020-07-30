using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public Action<int> OnPlayKey, OnPlayKeyDown, OnPlayKeyUp;
    public Action<int> OnSpeedKeyDown;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        for (int i = 0; i < CONST.PLAYKEYCODES.Length; i++)
        {
            KeyCode key = CONST.PLAYKEYCODES[i];

            if (Input.GetKey(key))
            {
                OnPlayKey?.Invoke(i);
            }

            if (Input.GetKeyDown(key))
            {
                OnPlayKeyDown?.Invoke(i);
            }

            if (Input.GetKeyUp(key))
            {
                OnPlayKeyUp?.Invoke(i);
            }
        }

        for (int i = 0; i < CONST.SPEEDKEYCODES.Length; i++)
        {
            KeyCode key = CONST.SPEEDKEYCODES[i];
            if (Input.GetKeyDown(key))
            {
                OnSpeedKeyDown?.Invoke(i);
            }
        }
    }
}
