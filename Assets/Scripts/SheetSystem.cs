using System;
using System.Collections.Generic;
using UnityEngine;

public class SheetSystem : MonoBehaviour
{
    #region INSPECTOR

    public Transform notesParent;
    public GameObject notePrefab;

    #endregion

    private List<List<NoteSystem>> noteSystemsByLine;
    private List<Queue<NoteSystem>> noteSystemQs;

    private void Awake()
    {
        noteSystemsByLine = new List<List<NoteSystem>>();
        noteSystemQs = new List<Queue<NoteSystem>>();
        for (int i = 0; i < 4; i++)
        {
            noteSystemsByLine.Add(new List<NoteSystem>());
        }

        GameManager.OnCallSheet += PrepareNotes;
        GameManager.OnCallSheet += () => { InputManager.Instance.OnPlayKeyDown += JudgePlayKeyDown; };
    }

    private void PrepareNotes()
    {
        GameManager.EndTime = 3f;

        foreach (Note item in GameManager.CurrentSheet.notes)
        {
            NoteSystem noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();
            noteSystemsByLine[item.line].Add(noteSystem);
            noteSystem.note = item;
            noteSystem.time = (float)(item.beat * (1f / GameManager.CurrentSheet.bpm) * 60f);
            GameManager.EndTime = Math.Max(GameManager.EndTime, noteSystem.time);
        }

        foreach (List<NoteSystem> item in noteSystemsByLine)
        {
            item.Sort();
            noteSystemQs.Add(new Queue<NoteSystem>(item));
        }

        GameManager.EndTime += 2f;
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
                GameManager.Instance.ActOnJudge(JUDGES.BREAK, gap);
                RemoveOneFromQ(i);
            }

        }
    }

    private void JudgePlayKeyDown(int key)
    {
        if (noteSystemQs[key].Count == 0)
        {
            return;
        }

        NoteSystem target = noteSystemQs[key].Peek();
        float gap = target.time - GameManager.CurrentTime;
        float absGap = Math.Abs(gap);

        if (absGap > GameManager.JUDGESTD[(int)JUDGES.BAD]) // DONT CARE
        {
            return;
        }

        if (absGap > GameManager.JUDGESTD[(int)JUDGES.NICE])
        {
            GameManager.Instance.ActOnJudge(JUDGES.BAD, gap);
        }
        else if (absGap > GameManager.JUDGESTD[(int)JUDGES.GREAT])
        {
            GameManager.Instance.ActOnJudge(JUDGES.NICE, gap);
        }
        else if (absGap > GameManager.JUDGESTD[(int)JUDGES.PRECISE])
        {
            GameManager.Instance.ActOnJudge(JUDGES.GREAT, gap);
        }
        else
        {
            GameManager.Instance.ActOnJudge(JUDGES.PRECISE, gap);
        }

        RemoveOneFromQ(key);
    }

    private void RemoveOneFromQ(int index)
    {
        NoteSystem target = noteSystemQs[index].Dequeue();
        Destroy(target.gameObject);
    }

    private bool Translate(SerializableInfo info, SerializableSheet sheet)
    {
        return true;
    }
}
