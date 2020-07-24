using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public Action<int> OnPlayKey = _ => { }, OnPlayKeyDown = _ => { }, OnPlayKeyUp = _ => { };
    public Action<int> OnSpeedKeyDown = _ => { };

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
                OnPlayKey(i);
            }

            if (Input.GetKeyDown(key))
            {
                OnPlayKeyDown(i);
            }

            if (Input.GetKeyUp(key))
            {
                OnPlayKeyUp(i);
            }
        }

        for (int i = 0; i < CONST.SPEEDKEYCODES.Length; i++)
        {
            KeyCode key = CONST.SPEEDKEYCODES[i];
            if (Input.GetKeyDown(key))
            {
                OnSpeedKeyDown(i);
            }
        }
    }
}
