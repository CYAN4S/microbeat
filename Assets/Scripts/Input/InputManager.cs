using System;
using System.Collections.Generic;
using FileIO;
using SO.NormalChannel;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;

        [Header("Channel to get values from previous scene")]
        [SerializeField] private ChartEventChannelSO onChartSelect;

        private const Key PauseKey = Key.Escape;

        private int current = -1;

        private Action read;

        private PlayerInput playerInput;

        private Binding _binding;

        private List<Key> speedKeys;
        private List<Key> playKeys;

        private void Awake()
        {
            var linex = onChartSelect.value?.pattern?.line;
            if (linex == null)
            {
                Debug.LogError("Playing Gameplay scene directly is not supported yet.");
                return;
            }
            var line = (int)linex;
            
            _binding = FileExplorer.FromFile<Binding>(Binding.Path) ?? Binding.Default();
            Debug.Log(JsonUtility.ToJson(_binding));
            
            speedKeys = _binding[line].Speed;
            playKeys = _binding[line].Play;

            read = line switch
            {
                4 => ReadOneByOne,
                6 => ReadOneByOne,
                8 => ReadOneByOne,
                5 => ReadSpecial,
                7 => ReadSpecial,
                _ => read
            };

            if (playKeys == null)
            {
                Debug.LogError("Unsupported Key.");
            }
        }

        private void Update()
        {
            read();
            
            for (var i = 0; i < speedKeys.Count; i++)
            {
                var key = speedKeys[i];
                if (Keyboard.current[key].wasPressedThisFrame) inputReader.OnSpeed(i);
            }
            
            if (Keyboard.current[PauseKey].wasPressedThisFrame) inputReader.OnPause();
        }

        private void ReadOneByOne()
        {
            for (var i = 0; i < playKeys.Count; i++)
            {
                var key = playKeys[i];
                if (Keyboard.current[key].isPressed) inputReader.OnPlayKey(i);

                if (Keyboard.current[key].wasPressedThisFrame) inputReader.OnPlayKeyDown(i);

                if (Keyboard.current[key].wasReleasedThisFrame) inputReader.OnPlayKeyUp(i);
            }
        }

        private void ReadSpecial()
        {
            for (var i = 0; i < 2; i++)
            {
                var key = playKeys[i];
                if (Keyboard.current[key].isPressed) inputReader.OnPlayKey(i);

                if (Keyboard.current[key].wasPressedThisFrame) inputReader.OnPlayKeyDown(i);

                if (Keyboard.current[key].wasReleasedThisFrame) inputReader.OnPlayKeyUp(i);
            }

            for (var i = 2; i < 4; i++)
            {
                var target = playKeys[i];
                if (Keyboard.current[target].wasPressedThisFrame)
                {
                    if (current != -1)
                    {
                        inputReader.OnPlayKeyInterrupt(2);
                    }

                    current = i;
                    inputReader.OnPlayKeyDown(2);
                }

                if (Keyboard.current[target].isPressed)
                {
                    if (current == i)
                    {
                        inputReader.OnPlayKey(2);
                    }
                }

                if (Keyboard.current[target].wasReleasedThisFrame)
                {
                    if (current == i)
                    {
                        inputReader.OnPlayKeyUp(2);
                        current = -1;
                    }
                }
            }

            for (var i = 4; i < playKeys.Count; i++)
            {
                var key = playKeys[i];
                if (Keyboard.current[key].isPressed) inputReader.OnPlayKey(i - 1);

                if (Keyboard.current[key].wasPressedThisFrame) inputReader.OnPlayKeyDown(i - 1);

                if (Keyboard.current[key].wasReleasedThisFrame) inputReader.OnPlayKeyUp(i - 1);
            }
        }
    }
}