using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    #region INSPECTOR
    public Transform notesParent;
    public GameObject notePrefab;
    public Sprite[] noteSprites;
    #endregion

    private List<Queue<NoteSystem>> noteSystemQs;
    private List<bool> isInLongNote;

    private void Awake()
    {
        noteSystemQs = new List<Queue<NoteSystem>>();
        isInLongNote = new List<bool>();
    }

    private void Start()
    {
        GameManager.instance.OnGameStart += () =>
        {
            PrepareNotes();

            InputManager.Instance.OnPlayKeyDown += JudgePlayKeyDown;
            InputManager.Instance.OnPlayKey += JudgePlayKey;
            InputManager.Instance.OnPlayKeyUp += JudgePlayKeyUp;
        };
    }

    private void PrepareNotes()
    {
        GameManager.EndTime = 3f;
        List<List<NoteSystem>> sortReady = new List<List<NoteSystem>>();
        for (int i = 0; i < 4; i++)
        {
            sortReady.Add(new List<NoteSystem>());
            isInLongNote.Add(false);
        }

        foreach (SerializableNote item in GameManager.instance.CurrentSheet.notes)
        {
            NoteSystem noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();

            noteSystem.SetFromData(item);
            noteSystem.time = (float)(item.beat * (1f / GameManager.instance.CurrentSheet.bpm) * 60f);

            noteSystem.GetComponent<Image>().sprite = noteSprites[(item.line == 1 || item.line == 2) ? 1 : 0];

            GameManager.EndTime = Math.Max(GameManager.EndTime, noteSystem.time);
            sortReady[item.line].Add(noteSystem);
        }

        foreach (List<NoteSystem> item in sortReady)
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
                GameManager.instance.HandleJudge(i, JUDGES.BREAK, gap);
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

        NoteSystem peek = noteSystemQs[key].Peek();
        float gap = peek.time - GameManager.CurrentTime;
        
        if (gap > CONST.JUDGESTD[(int)JUDGES.BAD]) // DONT CARE
        {
            return;
        }

        if (peek.notecode == NOTECODE.LONGNOTE)
        {
            HandleLongNoteDown(key, gap);
            isInLongNote[key] = true;
        }
        else
        {
            HandleNote(key, gap);
        }
    }

    private void JudgePlayKey(int key)
    {
        if (!isInLongNote[key])
        {
            return;
        }
    }

    private void JudgePlayKeyUp(int key)
    {
        if (!isInLongNote[key])
        {
            return;
        }
    }

    private void RemoveOneFromQ(int index)
    {
        NoteSystem target = noteSystemQs[index].Dequeue();
        Destroy(target.gameObject);
    }

    private void HandleNote(int key, float gap)
    {
        GameManager.instance.HandleJudge(key, GetJudgeFormGap(gap), gap);
        RemoveOneFromQ(key);
    }

    private void HandleLongNoteDown(int key, float gap)
    {
        GameManager.instance.HandleJudge(key, GetJudgeFormGap(gap), gap);
    }

    private void HandleLongNote(int key)
    {


    }

    private void HandleLongNoteUp(int key, float gap)
    {

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
