using System;
using UnityEngine;

public class NoteSystem : MonoBehaviour, IComparable<NoteSystem>
{
    public int Line { get; protected set; }
    public double Beat { get; protected set; }
    public float time;
    // public NOTECODE notecode;

    protected RectTransform rt;

    public int CompareTo(NoteSystem other)
    {
        return Beat.CompareTo(other.Beat);
    }

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        // notecode = NOTECODE.NOTE;
    }

    public void SetFromData(SerializableNote data)
    {
        Line = data.line;
        Beat = data.beat;
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
        transform.localPosition = new Vector3(CONST.LINEXPOS[Line], GetCurrentYPos());
    }

    protected float GetCurrentYPos()
    {
        return (float)((Beat - GameManager.CurrentBeat) * (float)GameManager.ScrollSpeed * 36000f / GameManager.Instance.Meta.std);
    }
}

