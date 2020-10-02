﻿using System;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    #region INSPECTOR
    public Transform notesParent;

    public GameObject notePrefab;
    public GameObject longNotePrefab;

    public Sprite[] noteSprites;
    #endregion

    private List<Queue<NoteSystem>> noteQueues;
    public List<NoteState> noteStates;

    private void Awake()
    {
        noteQueues = new List<Queue<NoteSystem>>();
        noteStates = new List<NoteState>();
    }

    public void SetPlayKey()
    {
        InputManager.Instance.OnPlayKeyDown += JudgePlayKeyDown;
        InputManager.Instance.OnPlayKey += JudgePlayKey;
        InputManager.Instance.OnPlayKeyUp += JudgePlayKeyUp;
    }

    public void PrepareNotes(SerializableDesc desc, SerializableSheet sheet)
    {
        GameManager.EndTime = 3f;
        List<List<NoteSystem>> sortReady = new List<List<NoteSystem>>();
        for (int i = 0; i < 4; i++)
        {
            sortReady.Add(new List<NoteSystem>());
            noteStates.Add(new NoteState());
        }

        foreach (SerializableNote item in sheet.notes)
        {
            NoteSystem noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();

            noteSystem.SetFromData(item);
            noteSystem.time = GameManager.Instance.Meta.GetTime(item.beat);
            noteSystem.GetComponent<Image>().sprite = noteSprites[(item.line == 1 || item.line == 2) ? 1 : 0];

            GameManager.EndTime = Math.Max(GameManager.EndTime, noteSystem.time);
            sortReady[item.line].Add(noteSystem);
        }

        foreach (SerializableLongNote item in sheet.longNotes)
        {
            LongNoteSystem longNoteSystem = Instantiate(longNotePrefab, notesParent).GetComponent<LongNoteSystem>();

            longNoteSystem.SetFromData(item);
            longNoteSystem.time = GameManager.Instance.Meta.GetTime(item.beat);
            longNoteSystem.endTime = GameManager.Instance.Meta.GetTime(item.beat + item.length);

            longNoteSystem.GetComponent<Image>().sprite = noteSprites[(item.line == 1 || item.line == 2) ? 1 : 0];

            GameManager.EndTime = Math.Max(GameManager.EndTime, longNoteSystem.endTime);
            sortReady[item.line].Add(longNoteSystem);
        }

        foreach (List<NoteSystem> item in sortReady)
        {
            item.Sort();
            noteQueues.Add(new Queue<NoteSystem>(item));
        }

        GameManager.EndTime += 2f;
    }

    private void LateUpdate()
    {
        RemoveBreakNotes();
    }

    private void RemoveBreakNotes()
    {
        for (int i = 0; i < noteQueues.Count; i++)
        {
            if (noteStates[i].isIn)
            {
                continue;
            }

            if (noteQueues[i].Count == 0)
            {
                continue;
            }

            float gap = GameManager.CurrentTime - noteQueues[i].Peek().time;
            if (gap > CONST.JUDGESTD[(int)JUDGES.BAD])
            {
                GameManager.Instance.ApplyBreak(i);
                DequeueNote(i);
            }
        }
    }

    private void JudgePlayKeyDown(int key)
    {
        if (noteQueues[key].Count == 0)
        {
            return;
        }

        NoteSystem peek = noteQueues[key].Peek();
        float gap = peek.time - GameManager.CurrentTime;

        if (gap > CONST.JUDGESTD[(int)JUDGES.BAD]) // DONT CARE
        {
            return;
        }

        if (peek.CompareTag("LongNote"))
        {
            noteStates[key].isIn = true;
            noteStates[key].startBeat = GameManager.CurrentBeat;
            noteStates[key].target = peek as LongNoteSystem;
            HandleLongNoteDown(key, gap);
        }
        else
        {
            HandleNote(key, gap);
        }
    }

    private void JudgePlayKey(int key)
    {
        if (!noteStates[key].isIn)
        {
            return;
        }

        HandleLongNoteTick(key);
    }

    private void JudgePlayKeyUp(int key)
    {
        if (!noteStates[key].isIn)
        {
            return;
        }

        HandleLongNoteUp(key);
    }

    private void DequeueNote(int index) => Destroy(noteQueues[index].Dequeue().gameObject);

    private void HandleNote(int key, float gap)
    {
        GameManager.Instance.ApplyNote(key, GetJudgeFormGap(gap), gap);
        DequeueNote(key);
    }

    private void HandleLongNoteDown(int key, float gap)
    {
        var temp = GetJudgeFormGap(gap);
        GameManager.Instance.ApplyLongNoteStart(key, temp, gap);
        noteStates[key].judge = temp == JUDGES.BAD ? JUDGES.NICE : temp;
    }

    private void HandleLongNoteTick(int key)
    {
        NoteState state = noteStates[key];
        state.target.isIn = true;

        if (state.target.endTime + CONST.JUDGESTD[(int)JUDGES.NICE] <= GameManager.CurrentTime)
        {
            GameManager.Instance.ApplyNote(key, JUDGES.NICE, CONST.JUDGESTD[(int)JUDGES.NICE]);
            state.Reset();
            DequeueNote(key);
            return;
        }

        if (state.target.ticks.Count == 0)
        {
            return;
        }

        if (state.target.ticks.Peek() + state.startBeat <= GameManager.CurrentBeat)
        {
            GameManager.Instance.ApplyLongNoteTick(key, state.judge);
            state.target.ticks.Dequeue();
        }
    }

    private void HandleLongNoteUp(int key)
    {
        NoteState state = noteStates[key];

        float gap = GameManager.CurrentTime - state.target.endTime;
        JUDGES j = GetJudgeFormGap(gap) != JUDGES.BAD ? state.judge : JUDGES.BAD;
        GameManager.Instance.ApplyLongNoteEnd(key, j, gap);
        state.Reset();
        DequeueNote(key);
    }

    private JUDGES GetJudgeFormGap(float gap)
    {
        float absGap = Math.Abs(gap);
        if (absGap > CONST.JUDGESTD[(int)JUDGES.NICE])
        {
            return JUDGES.BAD;
        }
        else if (absGap > CONST.JUDGESTD[(int)JUDGES.GREAT])
        {
            return JUDGES.NICE;
        }
        else if (absGap > CONST.JUDGESTD[(int)JUDGES.PRECISE])
        {
            return JUDGES.GREAT;
        }
        else
        {
            return JUDGES.PRECISE;
        }
    }


}

[Serializable]
public class NoteState
{
    public bool isIn;
    public double startBeat;
    public JUDGES judge;
    public LongNoteSystem target;

    public NoteState()
    {
        Reset();
    }

    public void Reset()
    {
        isIn = false;
        startBeat = 0;
        judge = JUDGES.BREAK;
        target = null;
    }
}
