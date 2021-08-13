using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SerializableData
{
    public int version;
}

/// <summary>
///     JSON File with ".mubd" extension.
///     bpms: [beat, bpm]
/// </summary>
[Serializable]
public class SerializableDescriptionData : SerializableData
{
    public string name;
    public string artist;
    public string genre;
    public double stdBpm;
    public List<List<double>> bpms;
    public string musicPath;
    public string previewImgPath;
    public string smallImgPath;
    public string imgPath;
    public string mvPath;

    public bool IsValid()
    {
        return bpms.All(bpm => bpm.Count == 2) && bpms.Count > 0;
    }

    public static SerializableDescriptionData UpgradeFrom(SerializableDesc old)
    {
        return new SerializableDescriptionData
        {
            name = old.name, artist = old.artist, genre = old.genre, musicPath = old.musicPath, stdBpm = old.bpm,
            previewImgPath = old.previewImgPath, smallImgPath = old.smallImgPath, imgPath = old.imgPath,
            mvPath = old.mvPath,
            bpms = old.bpms.Count > 0
                ? old.bpms.Select(item => new List<double> {item.beat, item.bpm}).ToList()
                : new List<List<double>>{new List<double>{0, old.bpm}}
        };
    }
}

/// <summary>
///     JSON File with ".mubp" extension.
///     notes: [line, beat]
///     longNotes: [line, beat, length]
/// </summary>
[Serializable]
public class SerializablePatternData : SerializableData
{
    public int line;
    public int level;
    public int diff;

    public List<List<double>> notes;
    public List<List<double>> longNotes;

    public bool IsValid()
    {
        return notes.All(note => note.Count == 2) && longNotes.All(longNote => longNote.Count == 3);
    }

    public static SerializablePatternData UpgradeFrom(SerializablePattern old)
    {
        return new SerializablePatternData
        {
            line = old.line, level = old.level, diff = old.diff,
            notes = old.notes.Select(item => new List<double> {item.line, item.beat}).ToList(),
            longNotes = old.longNotes.Select(item => new List<double> {item.line, item.beat, item.length}).ToList()
        };
    }
}

[Serializable]
public class SerializableDesc
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