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

    public AudioClip audioClip;

    #endregion

    private void Awake()
    {
        for (int i = 32; i < 48; i += 4)
        {
            notes.Add(new Note { beat = i, line = 0 });
            notes.Add(new Note { beat = i + 1, line = 1 });
            notes.Add(new Note { beat = i + 2, line = 0 });
            notes.Add(new Note { beat = i + 3, line = 1 });
            notes.Add(new Note { beat = i, line = 2 });
            notes.Add(new Note { beat = i + 0.5, line = 3 });
            notes.Add(new Note { beat = i + 0.75, line = 3 });
            notes.Add(new Note { beat = i + 1, line = 2 });
            notes.Add(new Note { beat = i + 1.5, line = 3 });
            notes.Add(new Note { beat = i + 2, line = 2 });
            notes.Add(new Note { beat = i + 2.25, line = 3 });
            notes.Add(new Note { beat = i + 2.5, line = 2 });
            notes.Add(new Note { beat = i + 3, line = 3 });
            notes.Add(new Note { beat = i + 3.5, line = 2 });
        }

        for (int i = 48; i < 64; i += 4)
        {
            notes.Add(new Note { beat = i, line = 3 });
            notes.Add(new Note { beat = i + 1, line = 2 });
            notes.Add(new Note { beat = i + 2, line = 3 });
            notes.Add(new Note { beat = i + 3, line = 2 });
            notes.Add(new Note { beat = i, line = 1 });
            notes.Add(new Note { beat = i + 0.5, line = 0 });
            notes.Add(new Note { beat = i + 0.75, line = 0 });
            notes.Add(new Note { beat = i + 1, line = 1 });
            notes.Add(new Note { beat = i + 1.5, line = 0 });
            notes.Add(new Note { beat = i + 2, line = 1 });
            notes.Add(new Note { beat = i + 2.25, line = 0 });
            notes.Add(new Note { beat = i + 2.5, line = 1 });
            notes.Add(new Note { beat = i + 3, line = 0 });
            notes.Add(new Note { beat = i + 3.5, line = 1 });
        }

        for (int i = 64; i < 128; i += 16)
        {
            notes.Add(new Note { beat = i, line = 2 });
            notes.Add(new Note { beat = i + 0.75, line = 2 });
            notes.Add(new Note { beat = i + 1.5, line = 2 });
            notes.Add(new Note { beat = i + 2.25, line = 2 });
            notes.Add(new Note { beat = i + 3, line = 2 });
            notes.Add(new Note { beat = i + 3.5, line = 3 });
            notes.Add(new Note { beat = i + 4, line = 1 });
            notes.Add(new Note { beat = i + 4.75, line = 1 });
            notes.Add(new Note { beat = i + 5.5, line = 1 });
            notes.Add(new Note { beat = i + 6.25, line = 1 });
            notes.Add(new Note { beat = i + 7, line = 1 });
            notes.Add(new Note { beat = i + 7.5, line = 3 });
            notes.Add(new Note { beat = i + 8, line = 2 });
            notes.Add(new Note { beat = i + 8.75, line = 2 });
            notes.Add(new Note { beat = i + 9.5, line = 2 });
            notes.Add(new Note { beat = i + 10.25, line = 2 });
            notes.Add(new Note { beat = i + 11, line = 2 });
            notes.Add(new Note { beat = i + 11.5, line = 1 });
            notes.Add(new Note { beat = i + 12, line = 2 });
            notes.Add(new Note { beat = i + 12.75, line = 2 });
            notes.Add(new Note { beat = i + 13.5, line = 2 });
            notes.Add(new Note { beat = i + 14.25, line = 3 });
            notes.Add(new Note { beat = i + 15, line = 3 });
            notes.Add(new Note { beat = i + 15.5, line = 3 });
        }

        for (int i = 96; i < 128; i += 1)
        {
            notes.Add(new Note { beat = i, line = 0 });
        }
    }


}

[Serializable]
public class SerializableSheet
{
    public double bpm;
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