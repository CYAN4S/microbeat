using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    public KeyCode[] speedKeys;
    public KeyCode[] playKeys4B;
    public KeyCode feverKey;
    
    private void Update()
    {
        for (int i = 0; i < playKeys4B.Length; i++)
        {
            var key = playKeys4B[i];
            if (Input.GetKey(key))
            {
                inputReader.OnPlayKey(i);
            }

            if (Input.GetKeyDown(key))
            {
                inputReader.OnPlayKeyDown(i);
            }

            if (Input.GetKeyUp(key))
            {
                inputReader.OnPlayKeyUp(i);
            }
        }

        for (int i = 0; i < speedKeys.Length; i++)
        {
            var key = speedKeys[i];
            if (Input.GetKeyDown(key))
            {
                inputReader.OnSpeed(i);
            }
        }
    }
}
