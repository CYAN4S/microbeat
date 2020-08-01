using System;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    #region INSPECTOR
    public Transform notesParent;
    public GameObject notePrefab;
    #endregion

    private GameManager gameManager;
    private List<List<NoteSystem>> noteSystemsByLine;
    private List<Queue<NoteSystem>> noteSystemQs;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();

        noteSystemsByLine = new List<List<NoteSystem>>();
        noteSystemQs = new List<Queue<NoteSystem>>();
        for (int i = 0; i < 4; i++)
        {
            noteSystemsByLine.Add(new List<NoteSystem>());
        }

        gameManager.OnGameStart += PrepareNotes;
        gameManager.OnGameStart += () => { InputManager.Instance.OnPlayKeyDown += JudgePlayKeyDown; };
    }

    private void PrepareNotes()
    {
        GameManager.EndTime = 3f;

        foreach (Note item in gameManager.CurrentSheet.notes)
        {
            NoteSystem noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();
            noteSystemsByLine[item.line].Add(noteSystem);
            noteSystem.note = item;
            noteSystem.time = (float)(item.beat * (1f / gameManager.CurrentSheet.bpm) * 60f);
            GameManager.EndTime = Math.Max(GameManager.EndTime, noteSystem.time);
            noteSystem.GetComponent<Animator>().SetTrigger(GetTriggerString(item.line));
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
            if (gap > CONST.JUDGESTD[(int)JUDGES.BAD])
            {
                gameManager.ActOnJudge(JUDGES.BREAK, gap);
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

        if (absGap > CONST.JUDGESTD[(int)JUDGES.BAD]) // DONT CARE
        {
            return;
        }

        if (absGap > CONST.JUDGESTD[(int)JUDGES.NICE])
        {
            gameManager.ActOnJudge(JUDGES.BAD, gap);
        }
        else if (absGap > CONST.JUDGESTD[(int)JUDGES.GREAT])
        {
            gameManager.ActOnJudge(JUDGES.NICE, gap);
        }
        else if (absGap > CONST.JUDGESTD[(int)JUDGES.PRECISE])
        {
            gameManager.ActOnJudge(JUDGES.GREAT, gap);
        }
        else
        {
            gameManager.ActOnJudge(JUDGES.PRECISE, gap);
        }

        RemoveOneFromQ(key);
    }

    private void RemoveOneFromQ(int index)
    {
        NoteSystem target = noteSystemQs[index].Dequeue();
        Destroy(target.gameObject);
    }

    private string GetTriggerString(int line)
    {
        if (line == 0 || line == 3)
        {
            return "ExecuteBlue";
        }
        else
        {
            return "ExecuteWhite";
        }
    }
}
