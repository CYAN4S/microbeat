using System.Collections.Generic;
using UnityEngine;

public class LongNoteSystem : NoteSystem
{
    public const float noteHeight = 50f;
    public float endTime;
    public double length;
    public Queue<double> ticks;
    public bool isIn;
    public bool pausedWhileIsIn = false;
    public double pausedTime;
    
    
    private float endPos;
    private float startPos;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        ticks = new Queue<double>();

        //GameManager.Instance.OnScrollSpeedChange += ChangeLength;
    }

    private void LateUpdate()
    {
        if (!player.IsWorking) return;

        GetPoses();
        Move();
        ChangeLength();
    }

    public void SetFromData(SerializableLongNote data)
    {
        Line = data.line;
        Beat = data.beat;
        length = data.length;
        for (var i = 0.25; i < length; i += 0.25) ticks.Enqueue(i);
    }

    private void ChangeLength()
    {
        var l = isIn ? endPos : endPos - startPos;
        l += noteHeight;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, l);
    }

    private void Move()
    {
        transform.localPosition = new Vector3(CONST.LINE_X_POS[Line], (startPos + endPos) / 2f);
    }

    private float GetCurrentEndYPos()
    {
        return GetYPos(Beat + length);
        // return (float) ((Beat + length - player.CurrentBeat) * (float) player.ScrollSpeed * 36000f / player.StdBpm);
    }

    private void GetPoses()
    {
        if (pausedWhileIsIn)
            startPos = Mathf.Max(GetYPos(pausedTime), 0);
        else if (isIn)
            startPos = 0;
        else
            startPos = GetCurrentYPos();
        
        endPos = GetCurrentEndYPos();
    }
}