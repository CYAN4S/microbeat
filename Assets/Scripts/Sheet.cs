using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheet : MonoBehaviour
{
    #region INSPECTOR
    public double bpm;
    public List<Note> notes;
    public List<LongNote> longNotes;

    #endregion
}

public interface INote : IComparable { }

[Serializable]
public class Note : INote
{
    public int line;
    public double timing;

    public int CompareTo(object obj)
    {
        if (obj is Note note)
        {
            return timing.CompareTo(note.timing);
        }
        throw new NotImplementedException();
    }
}

[Serializable]
public class LongNote : Note
{
    public double length;
}

[Serializable]
public class SerializableSheet
{
    public double bpm;
    public List<Note> notes;
    public List<LongNote> longNotes;
}
