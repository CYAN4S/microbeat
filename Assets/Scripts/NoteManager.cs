using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class NoteManager : MonoBehaviour
{
    #region INSPECTOR

    public Transform notesParent;
    public GameObject notePrefab;

    #endregion

    public static NoteManager Instance { get; private set; }

    private List<List<NoteSystem>> noteSystemsByLine;
    private List<Queue<NoteSystem>> noteSystemQs;

    private void Awake()
    {
        Instance = this;

        noteSystemsByLine = new List<List<NoteSystem>>();
        noteSystemQs = new List<Queue<NoteSystem>>();
        for (int i = 0; i < 4; i++)
        {
            noteSystemsByLine.Add(new List<NoteSystem>());
        }

        GameManager.OnCallSheet += PrepareNotes;
        GameManager.OnCallSheet += () => { InputManager.OnPlayKeyDown += JudgePlayKeyDown; };
    }

    private void PrepareNotes()
    {
        foreach (Note item in GameManager.CurrentSheet.notes)
        {
            var noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();
            noteSystemsByLine[item.line].Add(noteSystem);
            noteSystem.note = item;
            noteSystem.time = (float)(item.beat * (1f / GameManager.CurrentSheet.bpm) * 60f);
        }

        foreach (var item in noteSystemsByLine)
        {
            item.Sort();
            noteSystemQs.Add(new Queue<NoteSystem>(item));
        }

    }

    private void LateUpdate()
    {
        RemoveBreakNotes();
    }

    private void RemoveBreakNotes()
    {
        for (int i = 0; i < noteSystemQs.Count; i++)
        {
            Queue<NoteSystem> item = noteSystemQs[i];
            if (item.Count == 0)
            {
                continue;
            }

            NoteSystem target = item.Peek();
            float gap = GameManager.CurrentTime - target.time;
            if (gap > GameManager.JUDGESTD[(int)JUDGES.BAD])
            {
                GameManager.instance.ActOnJudge(JUDGES.BREAK, gap);
                RemoveOneFromQ(i);
            }

        }
    }

    private void JudgePlayKeyDown(int key)
    {
        if (noteSystemQs[key].Count == 0)
            return;

        NoteSystem target = noteSystemQs[key].Peek();
        float gap = target.time - GameManager.CurrentTime;
        float absGap = Math.Abs(gap);

        if (absGap > GameManager.JUDGESTD[(int)JUDGES.BAD]) // DONT CARE
        {
            return;
        }

        if (absGap > GameManager.JUDGESTD[(int)JUDGES.NICE])
        {
            GameManager.instance.ActOnJudge(JUDGES.BAD, gap);
        }
        else if (absGap > GameManager.JUDGESTD[(int)JUDGES.GREAT])
        {
            GameManager.instance.ActOnJudge(JUDGES.NICE, gap);
        }
        else if (absGap > GameManager.JUDGESTD[(int)JUDGES.PRECISE])
        {
            GameManager.instance.ActOnJudge(JUDGES.GREAT, gap);
        }
        else
        {
            GameManager.instance.ActOnJudge(JUDGES.PRECISE, gap);
        }

        RemoveOneFromQ(key);
    }

    private void RemoveOneFromQ(int index)
    {
        NoteSystem target = noteSystemQs[index].Dequeue();
        Destroy(target.gameObject);
    }
}
