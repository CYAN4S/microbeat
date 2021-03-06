﻿using System;
using System.Runtime.CompilerServices;
using FileIO;
using SO.NormalChannel;
using UnityEngine;

namespace Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;

        [Header("Channel to get values from previous scene")]
        [SerializeField] private ChartEventChannelSO onChartSelect;

        private KeyCode[] speedKeys;
        private const KeyCode PauseKey = KeyCode.Escape;

        private readonly (int, int)[] mul = {(2, 3), (3, 2)};
        private int current = -1;

        private KeyCode[] playKeys;
        private Action read;

        private void Awake()
        {
            var line = onChartSelect.value.pattern.line;
            
            var tmp = FileExplorer.FromFile<KeyBinding>(KeyBinding.Path) ?? KeyBinding.Default();
            var x = tmp[line];
            
            speedKeys = x.speedKeys;
            playKeys = x.playKeys;

            read = onChartSelect.value.pattern.line switch
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

            for (var i = 0; i < speedKeys.Length; i++)
            {
                var key = speedKeys[i];
                if (UnityEngine.Input.GetKeyDown(key)) inputReader.OnSpeed(i);
            }

            if (UnityEngine.Input.GetKeyDown(PauseKey)) inputReader.OnPause();
        }

        private void ReadOneByOne()
        {
            for (var i = 0; i < playKeys.Length; i++)
            {
                var key = playKeys[i];
                if (UnityEngine.Input.GetKey(key)) inputReader.OnPlayKey(i);

                if (UnityEngine.Input.GetKeyDown(key)) inputReader.OnPlayKeyDown(i);

                if (UnityEngine.Input.GetKeyUp(key)) inputReader.OnPlayKeyUp(i);
            }
        }

        private void ReadSpecial()
        {
            for (var i = 0; i < 2; i++)
            {
                var key = playKeys[i];
                if (UnityEngine.Input.GetKey(key)) inputReader.OnPlayKey(i);

                if (UnityEngine.Input.GetKeyDown(key)) inputReader.OnPlayKeyDown(i);

                if (UnityEngine.Input.GetKeyUp(key)) inputReader.OnPlayKeyUp(i);
            }

            for (var i = 2; i < 4; i++)
            {
                var target = playKeys[i];
                if (UnityEngine.Input.GetKeyDown(target))
                {
                    if (current != -1)
                    {
                        inputReader.OnPlayKeyInterrupt(2);
                    }

                    current = i;
                    inputReader.OnPlayKeyDown(2);
                }

                if (UnityEngine.Input.GetKey(target))
                {
                    if (current == i)
                    {
                        inputReader.OnPlayKey(2);
                    }
                }

                if (UnityEngine.Input.GetKeyUp(target))
                {
                    if (current == i)
                    {
                        inputReader.OnPlayKeyUp(2);
                        current = -1;
                    }
                }
            }

            for (var i = 4; i < playKeys.Length; i++)
            {
                var key = playKeys[i];
                if (UnityEngine.Input.GetKey(key)) inputReader.OnPlayKey(i - 1);

                if (UnityEngine.Input.GetKeyDown(key)) inputReader.OnPlayKeyDown(i - 1);

                if (UnityEngine.Input.GetKeyUp(key)) inputReader.OnPlayKeyUp(i - 1);
            }
        }
    }
}