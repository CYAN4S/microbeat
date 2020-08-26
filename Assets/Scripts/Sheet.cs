using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class Sheet
{
    public double bpm;
    public List<SerializableNote> notes;
    public List<SerializableLongNote> longNotes;
    public AudioClip audioClip;

    public Sheet(SerializableDesc desc, SerializableSheet sheet, AudioClip audioClip)
    {
        bpm = desc.bpm;
        notes = sheet.notes;
        this.audioClip = audioClip;
    }
}

[Serializable]
public class SerializableDesc
{
    public string name;
    public string artist;
    public string genre;

    public double bpm;

    public string musicPath;
}

[Serializable]
public class SerializableSheet
{
    public int line;
    public int level;
    public int pattern;

    public List<SerializableNote> notes;
    public List<SerializableLongNote> longNotes;
}

[Serializable]
public class SerializableNote
{
    public int line;
    public double beat;

    public int CompareTo(SerializableNote other) => beat.CompareTo(other.beat);
}

[Serializable]
public class SerializableLongNote : SerializableNote
{
    public double length;
}