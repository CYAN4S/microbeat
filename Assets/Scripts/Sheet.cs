using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class Sheet
{
    public double bpm;
    public List<SerializableBpm> bpms;
    public double endBeat;

    public List<SerializableNote> notes;
    public List<SerializableLongNote> longNotes;


    public Sheet(SerializableDesc desc, SerializableSheet sheet)
    {
        bpm = desc.bpm;
        bpms = desc.bpms;

        notes = sheet.notes;
        longNotes = sheet.longNotes;
    }
}

[Serializable]
public class SerializableDesc
{
    public string name;
    public string artist;
    public string genre;

    public double bpm;
    public List<SerializableBpm> bpms; // NEW VERSION
    public double endBeat; // NEW VERSION

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

[Serializable]
public class SerializableBpm
{
    public double beat;
    public double bpm;
}

public class BpmMeta
{
    public List<double> beat;
    public List<double> bpm;
    public List<double> length;
    public List<double> time;

    public BpmMeta(List<SerializableBpm> bpms, double endBeat)
    {
        GetMeta(bpms, endBeat);
    }

    public void GetMeta(List<SerializableBpm> bpms, double endBeat)
    {
        beat = new List<double>();
        bpm = new List<double>();
        length = new List<double>();
        time = new List<double>();

        beat.AddRange(from item in bpms select item.beat);
        bpm.AddRange(from item in bpms select item.bpm);
        beat.Add(endBeat);

        length.Add(60.0 * beat[1] * (1.0 / bpm[1]));
        time.Add(length[0]);

        for (int i = 1; i < bpms.Count; i++)
        {
            SerializableBpm item = bpms[i];
            length.Add(60.0 * (beat[i + 1] - beat[i]) * (1.0 / bpm[i]));
            time.Add(time[i - 1] + length[i]);
        }
    }

    
}