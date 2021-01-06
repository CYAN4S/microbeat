using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;

    public KeyCode[] SpeedKeys;
    public KeyCode[] PlayKeys;
    public KeyCode FeverKey;
    
    private void Update()
    {
        for (int i = 0; i < PlayKeys.Length; i++)
        {
            var key = PlayKeys[i];
            if (Input.GetKey(key))
            {
                _inputReader.OnPlayKey(i);
            }

            if (Input.GetKeyDown(key))
            {
                _inputReader.OnPlayKeyDown(i);
            }

            if (Input.GetKeyUp(key))
            {
                _inputReader.OnPlayKeyUp(i);
            }
        }

        for (int i = 0; i < SpeedKeys.Length; i++)
        {
            var key = SpeedKeys[i];
            if (Input.GetKeyDown(key))
            {
                _inputReader.OnSpeed(i);
            }
        }
    }
}
