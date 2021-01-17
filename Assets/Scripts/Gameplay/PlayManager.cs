using System;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    public List<NoteState> noteStates;

    [SerializeField] private PlayerSO player;


    [SerializeField] private InputReader inputReader;
    public Transform notesParent;

    public GameObject notePrefab;
    public GameObject longNotePrefab;

    public Sprite[] noteSprites;
    private List<Queue<NoteSystem>> noteQueues;

    private void Awake()
    {
        noteQueues = new List<Queue<NoteSystem>>();
        noteStates = new List<NoteState>();
    }

    private void LateUpdate()
    {
        RemoveBreakNotes();
    }

    private void OnEnable()
    {
        inputReader.playKeyEvent += JudgePlayKey;
        inputReader.playKeyDownEvent += JudgePlayKeyDown;
        inputReader.playKeyUpEvent += JudgePlayKeyUp;
    }

    private void OnDisable()
    {
        inputReader.playKeyEvent -= JudgePlayKey;
        inputReader.playKeyDownEvent -= JudgePlayKeyDown;
        inputReader.playKeyUpEvent -= JudgePlayKeyUp;
    }

    public void PrepareNotes(SerializableDesc desc, SerializablePattern pattern)
    {
        player.EndTime = 3f;
        var sortReady = new List<List<NoteSystem>>();
        for (var i = 0; i < 4; i++)
        {
            sortReady.Add(new List<NoteSystem>());
            noteStates.Add(new NoteState());
        }

        foreach (var item in pattern.notes)
        {
            var noteSystem = CreateNote(item);
            player.EndTime = Math.Max(player.EndTime, noteSystem.time);
            sortReady[item.line].Add(noteSystem);
        }

        foreach (var item in pattern.longNotes)
        {
            var longNoteSystem = Instantiate(longNotePrefab, notesParent).GetComponent<LongNoteSystem>();

            longNoteSystem.SetFromData(item);
            longNoteSystem.time = player.Meta.GetTime(item.beat);
            longNoteSystem.endTime = player.Meta.GetTime(item.beat + item.length);

            longNoteSystem.GetComponent<Image>().sprite = noteSprites[item.line == 1 || item.line == 2 ? 1 : 0];

            player.EndTime = Math.Max(player.EndTime, longNoteSystem.endTime);
            sortReady[item.line].Add(longNoteSystem);
        }

        foreach (var item in sortReady)
        {
            item.Sort();
            noteQueues.Add(new Queue<NoteSystem>(item));
        }

        player.EndTime += 2f;
    }

    private NoteSystem CreateNote(SerializableNote item)
    {
        var noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();

        noteSystem.SetFromData(item);
        noteSystem.time = player.Meta.GetTime(item.beat);
        noteSystem.GetComponent<Image>().sprite = noteSprites[item.line == 1 || item.line == 2 ? 1 : 0];

        return noteSystem;
    }

    private void RemoveBreakNotes()
    {
        for (var i = 0; i < noteQueues.Count; i++)
        {
            if (noteStates[i].isIn) continue;

            if (noteQueues[i].Count == 0) continue;

            var gap = player.CurrentTime - noteQueues[i].Peek().time;
            if (gap > CONST.JUDGE_STD[(int) JUDGES.BAD])
            {
                GameManager.Instance.ApplyBreak(i);
                DequeueNote(i);
            }
        }
    }

    private void JudgePlayKeyDown(int key)
    {
        if (!player.IsWorking) return;
        if (noteQueues[key].Count == 0) return;

        var peek = noteQueues[key].Peek();
        var gap = peek.time - player.CurrentTime;

        if (gap > CONST.JUDGE_STD[(int) JUDGES.BAD]) // DONT CARE
            return;

        if (peek.CompareTag("LongNote"))
        {
            noteStates[key].isIn = true;
            noteStates[key].startBeat = player.CurrentBeat;
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
        if (!player.IsWorking) return;
        if (!noteStates[key].isIn) return;

        HandleLongNoteTick(key);
    }

    private void JudgePlayKeyUp(int key)
    {
        if (!player.IsWorking) return;
        if (!noteStates[key].isIn) return;

        HandleLongNoteUp(key);
    }

    private void DequeueNote(int index)
    {
        Destroy(noteQueues[index].Dequeue().gameObject);
    }

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
        var state = noteStates[key];
        state.target.isIn = true;

        if (state.target.endTime + CONST.JUDGE_STD[(int) JUDGES.NICE] <= player.CurrentTime)
        {
            GameManager.Instance.ApplyNote(key, JUDGES.NICE, CONST.JUDGE_STD[(int) JUDGES.NICE]);
            state.Reset();
            DequeueNote(key);
            return;
        }

        if (state.target.ticks.Count == 0) return;

        if (state.target.ticks.Peek() + state.startBeat <= player.CurrentBeat)
        {
            GameManager.Instance.ApplyLongNoteTick(key, state.judge);
            state.target.ticks.Dequeue();
        }
    }

    private void HandleLongNoteUp(int key)
    {
        var state = noteStates[key];

        var gap = player.CurrentTime - state.target.endTime;
        var j = GetJudgeFormGap(gap) != JUDGES.BAD ? state.judge : JUDGES.BAD;
        GameManager.Instance.ApplyLongNoteEnd(key, j, gap);
        state.Reset();
        DequeueNote(key);
    }

    private JUDGES GetJudgeFormGap(float gap)
    {
        var absGap = Math.Abs(gap);
        if (absGap > CONST.JUDGE_STD[(int) JUDGES.NICE])
            return JUDGES.BAD;
        if (absGap > CONST.JUDGE_STD[(int) JUDGES.GREAT])
            return JUDGES.NICE;
        if (absGap > CONST.JUDGE_STD[(int) JUDGES.PRECISE])
            return JUDGES.GREAT;
        return JUDGES.PRECISE;
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