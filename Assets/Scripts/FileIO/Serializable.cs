using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SerializableData
{
    public int version;
}

[Serializable]
public class SerializableDesc : SerializableData
{
    public string name;
    public string artist;
    public string genre;

    public double bpm;
    public List<SerializableBpm> bpms;

    public string musicPath;
    public string previewImgPath;
    public string smallImgPath;
    public string imgPath;
    public string mvPath;
}

[Serializable]
public class SerializablePattern : SerializableData
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