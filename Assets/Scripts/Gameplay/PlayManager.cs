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

    private GameManager gm;

    private void Awake()
    {
        noteQueues = new List<Queue<NoteSystem>>();
        noteStates = new List<NoteState>();
        gm = GetComponent<GameManager>();
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
        player.GamePauseEvent += OnPause;
        player.GameResumeEvent += OnResume;
    }

    private void OnDisable()
    {
        inputReader.playKeyEvent -= JudgePlayKey;
        inputReader.playKeyDownEvent -= JudgePlayKeyDown;
        inputReader.playKeyUpEvent -= JudgePlayKeyUp;
        player.GamePauseEvent -= OnPause;
        player.GameResumeEvent -= OnResume;
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
            var longNoteSystem = CreateLongNote(item);
            player.EndTime = Math.Max(player.EndTime, longNoteSystem.endTime);
            sortReady[item.line].Add(longNoteSystem);
        }

        foreach (var item in sortReady)
        {
            item.Sort();
            noteQueues.Add(new Queue<NoteSystem>(item));
        }

        player.EndTime += 5f;
    }

    private NoteSystem CreateNote(SerializableNote item)
    {
        var noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();

        noteSystem.SetFromData(item);
        noteSystem.time = player.Meta.GetTime(item.beat);
        noteSystem.GetComponent<Image>().sprite = noteSprites[item.line == 1 || item.line == 2 ? 1 : 0];

        return noteSystem;
    }

    private LongNoteSystem CreateLongNote(SerializableLongNote item)
    {
        var longNoteSystem = Instantiate(longNotePrefab, notesParent).GetComponent<LongNoteSystem>();
        
        longNoteSystem.SetFromData(item);
        longNoteSystem.time = player.Meta.GetTime(item.beat);
        longNoteSystem.endTime = player.Meta.GetTime(item.beat + item.length);
        longNoteSystem.GetComponent<Image>().sprite = noteSprites[item.line == 1 || item.line == 2 ? 1 : 0];

        return longNoteSystem;
    }

    private void RemoveBreakNotes()
    {
        for (var i = 0; i < noteQueues.Count; i++)
        {
            if (noteStates[i].isInLongNote) continue;

            if (noteQueues[i].Count == 0) continue;

            float gap;
            gap = noteStates[i].pausedWhileIsIn
                ? player.CurrentTime - noteStates[i].target.pausedTime
                : player.CurrentTime - noteQueues[i].Peek().time;


            if (gap > CONST.JUDGE_STD[(int) JUDGES.BAD])
            {
                Debug.Log(noteStates[i].pausedWhileIsIn + " " + gap);
                gm.ApplyBreak(i);
                DequeueNote(i);
            }
        }
    }

    private void JudgePlayKeyDown(int key)
    {
        if (!player.IsWorking || player.IsPaused) return;
        if (noteQueues[key].Count == 0) return;

        var peek = noteQueues[key].Peek();
        var gap = peek.time - player.CurrentTime;

        if (gap > CONST.JUDGE_STD[(int) JUDGES.BAD]) // DONT CARE
            return;

        if (peek.CompareTag("LongNote"))
        {
            if (!noteStates[key].pausedWhileIsIn)
            {
                noteStates[key].isInLongNote = true;
                noteStates[key].startBeat = player.CurrentBeat;
                noteStates[key].target = peek as LongNoteSystem;
                noteStates[key].target.isIn = true;
                HandleLongNoteDown(key, gap);
            }
            else
            {
                noteStates[key].pausedWhileIsIn = false;
                noteStates[key].isInLongNote = true;

                noteStates[key].target.isIn = true;
                noteStates[key].target.pausedWhileIsIn = false;
                HandleLongNoteDownPausedWhileIsIn(key, gap);
            }
        }
        else
        {
            HandleNote(key, gap);
        }
    }

    private void JudgePlayKey(int key)
    {
        if (!player.IsWorking || player.IsPaused) return;
        if (!noteStates[key].isInLongNote) return;

        HandleLongNoteTick(key);
    }

    private void JudgePlayKeyUp(int key)
    {
        if (!player.IsWorking || player.IsPaused) return;
        if (!noteStates[key].isInLongNote) return;

        HandleLongNoteUp(key);
    }

    private void DequeueNote(int index)
    {
        Destroy(noteQueues[index].Dequeue().gameObject);
    }

    private void HandleNote(int key, float gap)
    {
        gm.ApplyNote(key, GetJudgeFormGap(gap), gap);
        DequeueNote(key);
    }

    private void HandleLongNoteDown(int key, float gap)
    {
        var temp = GetJudgeFormGap(gap);
        gm.ApplyLongNoteStart(key, temp, gap);
        noteStates[key].judge = temp == JUDGES.BAD ? JUDGES.NICE : temp;
    }

    private void HandleLongNoteDownPausedWhileIsIn(int key, float gap)
    {
        gm.ApplyLongNoteStartPausedWhileIsIn(key, noteStates[key].judge);
    }

    private void HandleLongNoteTick(int key)
    {
        var state = noteStates[key];

        if (state.target.endTime + CONST.JUDGE_STD[(int) JUDGES.NICE] <= player.CurrentTime)
        {
            gm.ApplyNote(key, JUDGES.NICE, CONST.JUDGE_STD[(int) JUDGES.NICE]);
            state.Reset();
            DequeueNote(key);
            return;
        }

        if (state.target.ticks.Count == 0) return;

        if (state.target.ticks.Peek() + state.startBeat <= player.CurrentBeat)
        {
            gm.ApplyLongNoteTick(key, state.judge);
            state.target.ticks.Dequeue();
        }
    }

    private void HandleLongNoteUp(int key)
    {
        var state = noteStates[key];

        var gap = player.CurrentTime - state.target.endTime;
        var j = GetJudgeFormGap(gap) != JUDGES.BAD ? state.judge : JUDGES.BAD;
        gm.ApplyLongNoteEnd(key, j, gap);
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

    private void OnPause()
    {
        Debug.Log(player.CurrentTime);
        foreach (var state in noteStates)
        {
            if (state.isInLongNote)
            {
                state.pausedWhileIsIn = true;
                state.isInLongNote = false;

                state.target.isIn = false;
                state.target.pausedWhileIsIn = true;
                state.target.pausedBeat = player.CurrentBeat;
                state.target.pausedTime = player.CurrentTime;
            }
        }
    }

    private void OnResume()
    {
        
    }
}

[Serializable]
public class NoteState
{
    public bool isInLongNote;
    public double startBeat;
    public JUDGES judge;
    public bool pausedWhileIsIn;
    
    public LongNoteSystem target;

    public NoteState()
    {
        Reset();
    }

    public void Reset()
    {
        isInLongNote = false;
        startBeat = 0;
        judge = JUDGES.BREAK;
        target = null;
        pausedWhileIsIn = false;
    }
}