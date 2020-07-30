using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class Sheet
{
    public double bpm;
    public List<Note> notes;
    public List<LongNote> longNotes;
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

    public List<Note> notes;
    public List<LongNote> longNotes;
}

[Serializable]
public class Note
{
    public int line;
    public double beat;

    public int CompareTo(Note other) => beat.CompareTo(other.beat);
}

[Serializable]
public class LongNote : Note
{
    public double length;
}