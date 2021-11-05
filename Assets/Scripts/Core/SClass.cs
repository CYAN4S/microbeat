using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Dictionary for serialization.
    /// </summary>
    /// <typeparam name="TK">Key Type</typeparam>
    /// <typeparam name="TV">Value Type</typeparam>
    [Serializable]
    public class SDictionary<TK, TV> : Dictionary<TK, TV>, ISerializationCallbackReceiver
        where TK : new() where TV : new()
    {
        [SerializeField] private List<TK> keys = new List<TK>();
        [SerializeField] private List<TV> values = new List<TV>();

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (var pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            if (keys.Count != values.Count)
                Debug.Log("Size of keys and values must be same!");
            for (var i = 0; i < Math.Max(keys.Count, values.Count); i++)
                Add(keys.Count > i ? keys[i] : new TK(), values.Count > i ? values[i] : new TV());
        }
    }

    /// <summary>
    /// List for multidimensional list serialization support.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    [Serializable]
    public class SList<T> : List<T>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<T> values = new List<T>();

        public void OnBeforeSerialize()
        {
            values.Clear();
            values.AddRange(this);
        }

        public void OnAfterDeserialize()
        {
            Clear();
            AddRange(values);
        }
    }
}