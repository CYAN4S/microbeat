using System;
using Gameplay;
using Gameplay.Visual;
using UnityEngine;

namespace SO.ParticularChannel
{
    [CreateAssetMenu(menuName = "Skin Collection", order = 0)]
    public class SkinCollection : ScriptableObject
    {
        public GearSet[] gearSet;
        public NoteSet[] noteSet;
    }


    [Serializable]
    public class GearSet
    {
        public string name;
        public SkinSystem gear4K;
        public SkinSystem gear5K;
        public SkinSystem gear6K;
        public SkinSystem gear8K;
    }

    [Serializable]
    public class NoteSet
    {
        public string name;
        public NoteSystem notePrefab;
        public NoteSystem notePrefabA;
        public LongNoteSystem longNotePrefab;
        public LongNoteSystem longNotePrefabA;
        public NoteSystem bumperNotePrefab;
        public LongNoteSystem bumperLongNotePrefab;
    }
}