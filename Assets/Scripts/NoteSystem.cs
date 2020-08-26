using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class NoteSystem : MonoBehaviour, IComparable<NoteSystem>
{
    public int Line { get; protected set; }
    public double Beat { get; protected set; }
    public float time;

    public int CompareTo(NoteSystem other) => Beat.CompareTo(other.Beat);

    public void SetFromData(SerializableNote data)
    {
        Line = data.line;
        Beat = data.beat;
    }

    private void Awake()
    {

    }

    private void LateUpdate()
    {
        if (!GameManager.IsWorking)
        {
            return;
        }

        Move();
    }

    private void Move()
    {
        transform.localPosition = new Vector3(CONST.LINEXPOS[Line], getCurrentYPos());
    }

    private float getCurrentYPos()
    {
        return (float)((time - GameManager.CurrentTime) * (float)GameManager.ScrollSpeed * 600f);
    }
}
