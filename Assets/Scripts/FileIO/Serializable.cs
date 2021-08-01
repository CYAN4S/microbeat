using System;
using System.Collections.Generic;
using System.Linq;

    [Obsolete]
    [Serializable]
    public class SerializableDesc
    {
        public string name;
        public string artist;
        public string genre;

        public double bpm;
        public List<SerializableBpm> bpms; // NEW VERSION

        public string musicPath;
        public string previewImgPath;
        public string smallImgPath;
        public string imgPath;
        public string mvPath;
    }

    [Obsolete]
    [Serializable]
    public class SerializablePattern
    {
        public int line;
        public int level;
        public int diff;

        public List<SerializableNote> notes;
        public List<SerializableLongNote> longNotes;
    }

    /// <summary>
    ///     JSON File with ".mubp" extension.
    /// </summary>
    [Serializable]
    public class SerializablePatternData
    {
        public int line;
        public int level;
        public int diff;

        public List<List<int>> notes;
        public List<List<int>> longNotes;

        public bool IsValid()
        {
            return notes.All(note => note.Count == 2) && longNotes.All(longNote => longNote.Count == 3);
        }
    }

    [Serializable]
    [Obsolete]
    public class SerializableNote
    {
        public int line;
        public double beat;

        public int CompareTo(SerializableNote other)
        {
            return beat.CompareTo(other.beat);
        }
    }

    [Serializable]
    [Obsolete]
    public class SerializableLongNote : SerializableNote
    {
        public double length;
    }

    [Serializable]
    [Obsolete]
    public class SerializableBpm
    {
        public double beat;
        public double bpm;
    }