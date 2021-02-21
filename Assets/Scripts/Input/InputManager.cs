using System;
using System.Runtime.CompilerServices;
using SO.NormalChannel;
using UnityEngine;

namespace Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        
        [Header("Channel to get values from previous scene")]
        [SerializeField] private ChartEventChannelSO onChartSelect;

        public KeyCode[] speedKeys;
        public KeyCode[] playKeys4B, playKeys5B, playKeys6B, playKeys8B;
        public KeyCode feverKey;
        public KeyCode pauseKey;

        private KeyCode[] playKeys;

        private void Awake()
        {
            playKeys = onChartSelect.value.pattern.line switch
            {
                4 => playKeys4B,
                5 => playKeys5B,
                6 => playKeys6B,
                8 => playKeys8B,
                _ => playKeys
            };
            if (playKeys == null)
            {
                Debug.LogError("Unsupported Key.");
            }
        }

        private void Update()
        {
            for (var i = 0; i < playKeys.Length; i++)
            {
                var key = playKeys[i];
                if (UnityEngine.Input.GetKey(key)) inputReader.OnPlayKey(i);

                if (UnityEngine.Input.GetKeyDown(key)) inputReader.OnPlayKeyDown(i);

                if (UnityEngine.Input.GetKeyUp(key)) inputReader.OnPlayKeyUp(i);
            }

            for (var i = 0; i < speedKeys.Length; i++)
            {
                var key = speedKeys[i];
                if (UnityEngine.Input.GetKeyDown(key)) inputReader.OnSpeed(i);
            }

            if (UnityEngine.Input.GetKeyDown(pauseKey)) inputReader.OnPause();
        }
    }
}