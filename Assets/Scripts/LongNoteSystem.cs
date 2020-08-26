using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteSystem : NoteSystem
{
    public double Length { get; protected set; }

    private void Awake()
    {
        GameManager.instance.OnScrollSpeedChange += ChangeLength;
    }

    public void SetFromData(SerializableLongNote data)
    {
        Line = data.line;
        Beat = data.beat;
        Length = data.length;
    }

    private void ChangeLength()
    {

    }
}
