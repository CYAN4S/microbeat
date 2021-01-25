using System;
using System.Collections.Generic;

namespace FileIO
{
    [Serializable]
    public class SerializableDesc
    {
        public string name;
        public string artist;
        public string genre;

        public double bpm;
        public List<SerializableBpm> bpms; // NEW VERSION

        public string musicPath;
    }

    [Serializable]
    public class SerializablePattern
    {
        public int line;
        public int level;
        public int diff;

        public List<SerializableNote> notes;
        public List<SerializableLongNote> longNotes;
    }

    [Serializable]
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
    public class SerializableLongNote : SerializableNote
    {
        public double length;
    }

    [Serializable]
    public class SerializableBpm
    {
        public double beat;
        public double bpm;
    }
}