using System;
using System.IO;
using UnityEngine;

namespace FileIO
{
    [Serializable]
    public class KeyBinding
    {
        public static readonly string PATH = Path.Combine(Application.persistentDataPath, "keybinding.mcbcore");
        public KeyCode[] speedKeys;
        public KeyCode[] playKeys4B, playKeys5B, playKeys6B, playKeys7B, playKeys8B;

        public static KeyBinding Default()
        {
            return new KeyBinding
            {
                speedKeys = new[] {KeyCode.T, KeyCode.G, KeyCode.H, KeyCode.Y},
                playKeys4B = new[] {KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K},
                playKeys5B = new[] {KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L},
                playKeys6B = new[] {KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L},
                playKeys7B = new[]
                {
                    KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.Semicolon, KeyCode.Quote, KeyCode.Return,
                    KeyCode.Space, KeyCode.RightAlt
                },
                playKeys8B = new[]
                {
                    KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.Semicolon, KeyCode.Quote, KeyCode.Return,
                    KeyCode.Space, KeyCode.RightAlt
                }
            };
        }
    }
}