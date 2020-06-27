using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSystem : MonoBehaviour, IComparable<NoteSystem>
{
    public Note noteData;

    public int CompareTo(NoteSystem other)
    {
        return noteData.CompareTo(other.noteData);
    }

    private void Awake() 
    {
        if (noteData is LongNote)
        {
            GameManager.OnScrollSpeedChange += ChangeLength;
        }
    }

    private void LateUpdate()
    {
        if (!GameManager.IsWorking)
            return;

    }

    private void ChangeLength()
    {

    }
}
