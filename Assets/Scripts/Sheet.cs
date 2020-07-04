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