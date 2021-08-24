using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FileIO
{
    [Serializable]
    public class Binding : ISerializationCallbackReceiver
    {
        public static string Path => System.IO.Path.Combine(Application.persistentDataPath, ".mubb");
        
        [Serializable]
        public class BindingByMode
        {
            public Key[] Speed;
            public Key[] Play;
        }

        public Dictionary<int, BindingByMode> Dict;

        [SerializeField] private List<int> keys = new List<int>();
        [SerializeField] private List<BindingByMode> values = new List<BindingByMode>();

        public static Binding Default()
        {
            return new Binding
            {
                Dict = new Dictionary<int, BindingByMode>
                {
                    {
                        4, new BindingByMode
                        {
                            Speed = new[] {Key.T, Key.G, Key.H, Key.Y},
                            Play = new[] {Key.D, Key.F, Key.J, Key.K}
                        }
                    },
                    {
                        5, new BindingByMode
                        {
                            Speed = new[] {Key.T, Key.G, Key.H, Key.Y},
                            Play = new[] {Key.S, Key.D, Key.F, Key.J, Key.K, Key.L}
                        }
                    },
                    {
                        6, new BindingByMode
                        {
                            Speed = new[] {Key.T, Key.G, Key.H, Key.Y},
                            Play = new[] {Key.S, Key.D, Key.F, Key.J, Key.K, Key.L}
                        }
                    },
                    {
                        8, new BindingByMode
                        {
                            Speed = new[] {Key.T, Key.G, Key.H, Key.Y},
                            Play = new[]
                                {Key.S, Key.D, Key.F, Key.Semicolon, Key.Quote, Key.Enter, Key.Space, Key.RightAlt}
                        }
                    },
                }
            };
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (var kv in Dict)
            {
                keys.Add(kv.Key);
                values.Add(kv.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Dict.Clear();
            for (int i = 0; i < keys.Count; i++)
            {
                Dict.Add(keys[i], values[i]);
            }
        }
    }


    [Serializable]
    public class KeyBinding
    {
        public static string Path => System.IO.Path.Combine(Application.persistentDataPath, "keybinding.mcbcore");

        public KeyCode[] speedKeys4K, speedKeys5K, speedKeys6K, speedKeys8K;
        public KeyCode[] playKeys4K, playKeys5K, playKeys6K, playKeys8K;

        public class KeyPair
        {
            public KeyCode[] playKeys;
            public KeyCode[] speedKeys;

            public KeyPair(KeyCode[] playKeys, KeyCode[] speedKeys)
            {
                this.playKeys = playKeys;
                this.speedKeys = speedKeys;
            }
        }

        public static KeyBinding Default()
        {
            return new KeyBinding
            {
                speedKeys4K = new[] {KeyCode.T, KeyCode.G, KeyCode.H, KeyCode.Y},
                speedKeys5K = new[] {KeyCode.T, KeyCode.G, KeyCode.H, KeyCode.Y},
                speedKeys6K = new[] {KeyCode.T, KeyCode.G, KeyCode.H, KeyCode.Y},
                speedKeys8K = new[] {KeyCode.T, KeyCode.G, KeyCode.H, KeyCode.Y},
                playKeys4K = new[] {KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K},
                playKeys5K = new[] {KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L},
                playKeys6K = new[] {KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L},
                playKeys8K = new[]
                {
                    KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.Semicolon, KeyCode.Quote, KeyCode.Return,
                    KeyCode.Space, KeyCode.RightAlt
                }
            };
        }

        public KeyPair this[int key] => key switch
        {
            4 => new KeyPair(playKeys4K, speedKeys4K),
            5 => new KeyPair(playKeys5K, speedKeys5K),
            6 => new KeyPair(playKeys6K, speedKeys6K),
            8 => new KeyPair(playKeys8K, speedKeys8K),
            _ => throw new ArgumentException($"{key} is not in 4, 5, 6 or 8.")
        };
    }
}