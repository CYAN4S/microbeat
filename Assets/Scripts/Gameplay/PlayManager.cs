using System;
using System.Collections.Generic;
using Core;
using FileIO;
using Input;
using SO;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class PlayManager : MonoBehaviour
    {
        [Header("Requirement")]
        [SerializeField] private PlayerSO player;
        [SerializeField] private InputReader inputReader;
        
        [Header("Note Settings")]
        [SerializeField] private GameObject notePrefab;
        [SerializeField] private GameObject longNotePrefab;
        [SerializeField] private Transform noteContainer;
        [SerializeField] private Sprite[] noteSprites;

        private GameManager gm;
        private List<Queue<NoteSystem>> noteQueues;
        private List<NoteState> noteStates;
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
            var noteSystem = Instantiate(notePrefab, noteContainer).GetComponent<NoteSystem>();

            noteSystem.SetFromData(item);
            noteSystem.time = player.Meta.GetTime(item.beat);
            noteSystem.GetComponent<Image>().sprite = noteSprites[item.line == 1 || item.line == 2 ? 1 : 0];

            return noteSystem;
        }

        private LongNoteSystem CreateLongNote(SerializableLongNote item)
        {
            var longNoteSystem = Instantiate(longNotePrefab, noteContainer).GetComponent<LongNoteSystem>();

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


                if (gap > Const.JUDGE_STD[(int) Judges.Bad])
                {
                    gm.ApplyBreak(i);
                    DequeueNote(i);
                }
            }
        }

        private void JudgePlayKeyDown(int key)
        {
            if (!player.State.IsCatchable) return;
            if (noteQueues[key].Count == 0) return;

            var peek = noteQueues[key].Peek();
            var gap = peek.time - player.CurrentTime;

            if (gap > Const.JUDGE_STD[(int) Judges.Bad]) // DONT CARE
                return;

            if (peek.CompareTag("LongNote"))
            {
                if (!noteStates[key].pausedWhileIsIn)
                {
                    if (!player.State.IsPlayable) return;
                    
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
                    HandleCutOffLongNoteDown(key, gap);
                }
            }
            else
            {
                if (!player.State.IsPlayable) return;
                HandleNote(key, gap);
            }
        }

        private void JudgePlayKey(int key)
        {
            if (!player.State.IsPlayable) return;
            if (!noteStates[key].isInLongNote) return;

            HandleLongNoteTick(key);
        }

        private void JudgePlayKeyUp(int key)
        {
            if (!player.State.IsPlayable) return;
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
            noteStates[key].judge = temp == Judges.Bad ? Judges.Nice : temp;
        }

        private void HandleCutOffLongNoteDown(int key, float gap)
        {
            gm.ApplyCutOffLongNoteStart(key, noteStates[key].judge);
        }

        private void HandleLongNoteTick(int key)
        {
            var state = noteStates[key];

            if (state.target.endTime + Const.JUDGE_STD[(int) Judges.Nice] <= player.CurrentTime)
            {
                gm.ApplyNote(key, Judges.Nice, Const.JUDGE_STD[(int) Judges.Nice]);
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
            var j = GetJudgeFormGap(gap) != Judges.Bad ? state.judge : Judges.Bad;
            gm.ApplyLongNoteEnd(key, j, gap);
            state.Reset();
            DequeueNote(key);
        }

        private Judges GetJudgeFormGap(float gap)
        {
            var absGap = Math.Abs(gap);
            if (absGap > Const.JUDGE_STD[(int) Judges.Nice])
                return Judges.Bad;
            if (absGap > Const.JUDGE_STD[(int) Judges.Great])
                return Judges.Nice;
            if (absGap > Const.JUDGE_STD[(int) Judges.Precise])
                return Judges.Great;
            return Judges.Precise;
        }

        private void OnPause()
        {
            Debug.Log(player.CurrentTime);
            foreach (var state in noteStates)
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

        private void OnResume()
        {
        }
    }

    [Serializable]
    public class NoteState
    {
        public bool isInLongNote;
        public double startBeat;
        public Judges judge;
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
            judge = Judges.Break;
            target = null;
            pausedWhileIsIn = false;
        }
    }
}