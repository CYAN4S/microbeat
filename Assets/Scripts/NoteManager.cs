using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteManager : MonoBehaviour
{
    #region INSPECTOR
    public Transform notesParent;
    public GameObject notePrefab;
    public Sprite[] noteSprites;
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


    }

    private void Start()
    {
        GameManager.instance.OnGameStart += PrepareNotes;
        GameManager.instance.OnGameStart += () => { InputManager.Instance.OnPlayKeyDown += JudgePlayKeyDown; };
    }

    private void PrepareNotes()
    {
        GameManager.EndTime = 3f;

        foreach (SerializableNote item in GameManager.instance.CurrentSheet.notes)
        {
            NoteSystem noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();
            noteSystemsByLine[item.line].Add(noteSystem);
            noteSystem.SetFromData(item);
            noteSystem.time = (float)(item.beat * (1f / GameManager.instance.CurrentSheet.bpm) * 60f);
            GameManager.EndTime = Math.Max(GameManager.EndTime, noteSystem.time);
            noteSystem.GetComponent<Image>().sprite = noteSprites[(item.line == 1 || item.line == 2) ? 1 : 0];
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
                GameManager.instance.ActOnJudge(JUDGES.BREAK, gap);
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
            GameManager.instance.ActOnJudge(JUDGES.BAD, gap);
        }
        else if (absGap > CONST.JUDGESTD[(int)JUDGES.GREAT])
        {
            GameManager.instance.ActOnJudge(JUDGES.NICE, gap);
            GameManager.instance.ExplodeNote(key);
        }
        else if (absGap > CONST.JUDGESTD[(int)JUDGES.PRECISE])
        {
            GameManager.instance.ActOnJudge(JUDGES.GREAT, gap);
            GameManager.instance.ExplodeNote(key);
        }
        else
        {
            GameManager.instance.ActOnJudge(JUDGES.PRECISE, gap);
            GameManager.instance.ExplodeNote(key);
        }

        RemoveOneFromQ(key);
    }

    private void RemoveOneFromQ(int index)
    {
        NoteSystem target = noteSystemQs[index].Dequeue();
        Destroy(target.gameObject);
    }
}
