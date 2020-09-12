using System;
using System.Collections.Generic;
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

    private List<Queue<NoteSystem>> noteSystemQs;
    public List<LongNoteProcess> longNoteProcesses;

    private void Awake()
    {
        noteSystemQs = new List<Queue<NoteSystem>>();
        longNoteProcesses = new List<LongNoteProcess>();
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
            longNoteProcesses.Add(new LongNoteProcess());
        }

        foreach (SerializableNote item in GameManager.instance.Now.notes)
        {
            NoteSystem noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();

            noteSystem.SetFromData(item);
            //noteSystem.time = (float)(item.beat * (1f / GameManager.instance.Now.bpmMeta.std) * 60f); // OLD
            noteSystem.time = GameManager.instance.Now.bpmMeta.GetTime(item.beat);
            noteSystem.GetComponent<Image>().sprite = noteSprites[(item.line == 1 || item.line == 2) ? 1 : 0];

            GameManager.EndTime = Math.Max(GameManager.EndTime, noteSystem.time);
            sortReady[item.line].Add(noteSystem);
        }

        foreach (SerializableLongNote item in GameManager.instance.Now.longNotes)
        {
            LongNoteSystem longNoteSystem = Instantiate(longNotePrefab, notesParent).GetComponent<LongNoteSystem>();

            longNoteSystem.SetFromData(item);
            //longNoteSystem.time = (float)(item.beat * (1f / GameManager.instance.Now.bpmMeta.std) * 60f); // OLD
            longNoteSystem.time = GameManager.instance.Now.bpmMeta.GetTime(item.beat);
            //longNoteSystem.endTime = (float)((item.beat + item.length) * (1f / GameManager.instance.Now.bpmMeta.std) * 60f);
            longNoteSystem.endTime = GameManager.instance.Now.bpmMeta.GetTime(item.beat + item.length);

            longNoteSystem.GetComponent<Image>().sprite = noteSprites[(item.line == 1 || item.line == 2) ? 1 : 0];

            GameManager.EndTime = Math.Max(GameManager.EndTime, longNoteSystem.endTime);
            sortReady[item.line].Add(longNoteSystem);
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
            if (longNoteProcesses[i].isIn)
            {
                continue;
            }

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

        if (peek.CompareTag("LongNote"))
        {
            longNoteProcesses[key].isIn = true;
            longNoteProcesses[key].startBeat = GameManager.CurrentBeat;
            longNoteProcesses[key].target = peek as LongNoteSystem;
            HandleLongNoteDown(key, gap);
        }
        else
        {
            HandleNote(key, gap);
        }
    }

    private void JudgePlayKey(int key)
    {
        if (!longNoteProcesses[key].isIn)
        {
            return;
        }

        HandleLongNoteTick(key);
    }

    private void JudgePlayKeyUp(int key)
    {
        if (!longNoteProcesses[key].isIn)
        {
            return;
        }

        HandleLongNoteUp(key);
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
        longNoteProcesses[key].judge = GetJudgeFormGap(gap);
        GameManager.instance.HandleFirstTickJudge(key, longNoteProcesses[key].judge, gap);
    }

    private void HandleLongNoteTick(int key)
    {
        LongNoteProcess process = longNoteProcesses[key];
        process.target.isIn = true;

        if (process.target.endTime + CONST.JUDGESTD[(int)JUDGES.NICE] <= GameManager.CurrentTime)
        {
            GameManager.instance.HandleJudge(key, JUDGES.NICE, CONST.JUDGESTD[(int)JUDGES.NICE]);
            process.Reset();
            RemoveOneFromQ(key);
            return;
        }

        if (process.target.ticks.Count == 0)
        {
            return;
        }

        if (process.target.ticks.Peek() + process.startBeat <= GameManager.CurrentBeat)
        {
            GameManager.instance.HandleTickJudge(key, process.judge);
            process.target.ticks.Dequeue();
        }
    }

    private void HandleLongNoteUp(int key)
    {
        LongNoteProcess process = longNoteProcesses[key];

        float gap = GameManager.CurrentTime - process.target.endTime;
        JUDGES j = GetJudgeFormGap(gap);
        j = j != JUDGES.BAD ? process.judge : JUDGES.BAD;
        GameManager.instance.HandleJudge(key, j, gap);
        process.Reset();
        RemoveOneFromQ(key);
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
public class LongNoteProcess
{
    public bool isIn;
    public double startBeat;
    public JUDGES judge;
    public LongNoteSystem target;

    public LongNoteProcess()
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
