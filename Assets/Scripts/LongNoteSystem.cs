using System.Collections.Generic;
using UnityEngine;

public class LongNoteSystem : NoteSystem
{
    public float endTime;
    public static float noteHeight = 50f;
    public double length;
    public Queue<double> ticks;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        ticks = new Queue<double>();

        ChangeLength();
        GameManager.instance.OnScrollSpeedChange += ChangeLength;
        notecode = NOTECODE.LONGNOTE;
    }

    public void SetFromData(SerializableLongNote data)
    {
        Line = data.line;
        Beat = data.beat;
        length = data.length;
        for (double i = 0.25; i < length; i += 0.25)
        {
            ticks.Enqueue(i);
        }
    }

    private void ChangeLength()
    {
        float l = (float)(length * GameManager.ScrollSpeed * 36000f / GameManager.instance.CurrentSheet.bpm);
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, l);
    }

    private void LateUpdate()
    {
        if (!GameManager.IsWorking)
        {
            return;
        }

        Move();
        ChangeLength();
    }

    private void Move()
    {
        transform.localPosition = new Vector3(CONST.LINEXPOS[Line], (GetCurrentYPos() + GetCurrentEndYPos()) / 2f);
    }

    private float GetCurrentEndYPos()
    {
        return (float)((Beat + length - GameManager.CurrentBeat) * (float)GameManager.ScrollSpeed * 36000f / GameManager.instance.CurrentSheet.bpm);
    }
}
