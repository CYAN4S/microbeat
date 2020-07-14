using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public static readonly KeyCode[] PLAYKEYCODES = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };
    public static readonly KeyCode[] SPEEDKEYCODES = { KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I };

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
        for (int i = 0; i < PLAYKEYCODES.Length; i++)
        {
            KeyCode key = PLAYKEYCODES[i];

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

        for (int i = 0; i < SPEEDKEYCODES.Length; i++)
        {
            KeyCode key = SPEEDKEYCODES[i];
            if (Input.GetKeyDown(key))
            {
                OnSpeedKeyDown(i);
            }
        }
    }
}
