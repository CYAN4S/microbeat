using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class NoteSystem : MonoBehaviour, IComparable<NoteSystem>
{
    public Note note;
    public float time;

    public int CompareTo(NoteSystem other)
    {
        return note.CompareTo(other.note);
    }

    private void Awake()
    {
        if (note is LongNote)
        {
            GameManager.instance.OnScrollSpeedChange += ChangeLength;
        }
    }

    private void LateUpdate()
    {
        if (!GameManager.IsWorking)
        {
            return;
        }

        Move();
    }

    private void ChangeLength()
    {

    }

    private void Move()
    {
        transform.localPosition = new Vector3(CONST.LINEXPOS[note.line], getCurrentYPos());
    }

    private float getCurrentYPos()
    {
        return (float)(note.beat - GameManager.CurrentTime * GameManager.instance.CurrentSheet.bpm * (1f / 60f)) * 120f * 2f * (float)GameManager.ScrollSpeed;
    }
}
