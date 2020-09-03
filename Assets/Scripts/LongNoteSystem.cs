using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteSystem : NoteSystem
{
    public double Length { get; protected set; }
    public Queue<float> tickTimeDelta;

    public new readonly NOTECODE notecode = NOTECODE.LONGNOTE;

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

    private float getCurrentEndYPos()
    {
        return (float)((time - GameManager.CurrentTime) * (float)GameManager.ScrollSpeed * 600f);
    }
}
