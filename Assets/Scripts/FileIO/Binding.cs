using System;
using System.Linq;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FileIO
{
    public enum BindingEnum
    {
        Speed, Play
    }
    
    [Serializable]
    public class BindingByMode : SList<SList<Key>>
    {
        public SList<Key> Speed => this[0];
        public SList<Key> Play => this[1];

        public override string ToString()
        {
            var result = "\n";
            foreach (var keyList in this)
            {
                result = keyList.Aggregate(result, (current, key) => current + $"{key}\t");
                result += "\n";
            }
            return result;
        }
    }
    
    [Serializable]
    public class Binding : SDictionary<int, BindingByMode>
    {
        public static string Path => System.IO.Path.Combine(Application.persistentDataPath, ".mubb");

        public static Binding Default()
        {
            return new Binding
            {
                {
                    4, new BindingByMode
                    {
                        new SList<Key> {Key.T, Key.G, Key.H, Key.Y},
                        new SList<Key> {Key.D, Key.F, Key.J, Key.K}
                    }
                },
                {
                    5, new BindingByMode
                    {
                        new SList<Key> {Key.T, Key.G, Key.H, Key.Y},
                        new SList<Key> {Key.S, Key.D, Key.F, Key.J, Key.K, Key.L}
                    }
                },
                {
                    6, new BindingByMode
                    {
                        new SList<Key> {Key.T, Key.G, Key.H, Key.Y},
                        new SList<Key> {Key.S, Key.D, Key.F, Key.J, Key.K, Key.L}
                    }
                },
                {
                    8, new BindingByMode
                    {
                        new SList<Key> {Key.T, Key.G, Key.H, Key.Y},
                        new SList<Key> {Key.S, Key.D, Key.F, Key.Semicolon, Key.Quote, Key.Enter, Key.Space, Key.RightAlt}
                    }
                },
            };
        }
    }
}