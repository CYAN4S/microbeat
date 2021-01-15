using System;
using Events;
using UnityEngine;

public class NoteSystem : MonoBehaviour, IComparable<NoteSystem>
{
    public float time;

    protected RectTransform rt;
    public int Line { get; protected set; }
    public double Beat { get; protected set; }

    [SerializeField] protected PlayerSO player;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (!player.IsWorking) return;

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
        transform.localPosition = new Vector3(CONST.LINE_X_POS[Line], GetCurrentYPos());
    }

    protected float GetCurrentYPos()
    {
        return (float) ((Beat - player.CurrentBeat) * (float) player.ScrollSpeed * 36000f /
                        player.StdBpm);
    }
}