using System;
using UnityEngine;

public class NoteSystem : MonoBehaviour, IComparable<NoteSystem>
{
    public float time;

    protected RectTransform rt;
    public int Line { get; protected set; }
    public double Beat { get; protected set; }

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (!GameManager.IsWorking) return;

        Move();
    }

    public int CompareTo(NoteSystem other)
    {
        return Beat.CompareTo(other.Beat);
    }

    public void SetFromData(SerializableNote data)
    {
        Line = data.line;
        Beat = data.beat;
    }

    private void Move()
    {
        transform.localPosition = new Vector3(CONST.LINEXPOS[Line], GetCurrentYPos());
    }

    protected float GetCurrentYPos()
    {
        return (float) ((Beat - GameManager.CurrentBeat) * (float) GameManager.ScrollSpeed * 36000f /
                        GameManager.Instance.Meta.std);
    }
}