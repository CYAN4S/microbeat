using System.Collections.Generic;
using UnityEngine;

public class LongNoteSystem : NoteSystem
{
    public float endTime;
    public const float noteHeight = 50f;
    public double length;
    public Queue<double> ticks;
    public bool isIn = false;

    private float startPos;
    private float endPos;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        ticks = new Queue<double>();

        //GameManager.Instance.OnScrollSpeedChange += ChangeLength;
    }

    private void LateUpdate()
    {
        if (!GameManager.IsWorking)
        {
            return;
        }

        GetPoses();
        Move();
        ChangeLength();
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
        float l = isIn ? endPos : endPos - startPos;
        l += noteHeight;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, l);
    }

    private void Move()
    {
        transform.localPosition = new Vector3(CONST.LINEXPOS[Line], (startPos + endPos) / 2f);
    }

    private float GetCurrentEndYPos()
    {
        return (float)((Beat + length - GameManager.CurrentBeat) * (float)GameManager.ScrollSpeed * 36000f / GameManager.Instance.Meta.std);
    }

    private void GetPoses()
    {
        startPos = isIn ? 0 : GetCurrentYPos();
        endPos = GetCurrentEndYPos();
    }
}
