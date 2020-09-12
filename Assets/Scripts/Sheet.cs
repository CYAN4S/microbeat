using System;
using System.Collections.Generic;
using System.Linq;

public class Sheet
{
    public BpmMeta bpmMeta;

    public List<SerializableNote> notes;
    public List<SerializableLongNote> longNotes;


    public Sheet(SerializableDesc desc, SerializableSheet sheet)
    {
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

[Serializable]
public class BpmMeta
{
    public List<double> beats;
    public List<double> bpms;
    public List<double> lengths;
    public List<double> startTime;
    public List<double> endTimes;
    public double std;

    public BpmMeta(List<SerializableBpm> bpms, double stdBpm)
    {
        GetMeta(bpms);
        std = stdBpm;
    }

    public BpmMeta(double stdBpm)
    {
        List<SerializableBpm> b = new List<SerializableBpm>
        {
            new SerializableBpm()
            {
                beat = 0,
                bpm = stdBpm
            }
        };
        GetMeta(b);
        std = stdBpm;
    }

    public void GetMeta(List<SerializableBpm> bpmData)
    {
        // Init
        beats = new List<double>();
        bpms = new List<double>();
        lengths = new List<double>();
        endTimes = new List<double>();

        // Set beat, bpm
        beats.AddRange(from item in bpmData select item.beat);
        bpms.AddRange(from item in bpmData select item.bpm);

        // Calc length
        for (int i = 0; i < beats.Count - 1; i++)
        {
            lengths.Add(60.0 * (beats[i + 1] - beats[i]) / bpms[i]);
        }
        lengths.Add(double.MaxValue); // PLZ Don't rely on this value.

        // Calc endTime
        endTimes.Add(lengths[0]);
        for (int i = 1; i < beats.Count - 1; i++)
        {
            endTimes.Add(lengths[i] + endTimes[i - 1]);
        }
        endTimes.Add(double.MaxValue); // PLZ Don't rely on this value too.
    }

    // Don't use it too often.
    public float GetTime(double beat)
    {
        int index;
        for (index = 0; index < beats.Count - 1; index++)
        {
            if (beats[index + 1] >= beat)
            {
                break;
            }
        }

        if (index == 0)
        {
            return (float)((beat - beats[index]) * 60.0 / bpms[index]);
        }

        return (float)(endTimes[index - 1] + (beat - beats[index]) * 60.0 / bpms[index]);
    }
}