using System;
using System.Collections.Generic;
using Core;
using FileIO;
using Gameplay.Visual;
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
        
        [SerializeField] private NoteSystem notePrefab;
        [SerializeField] private NoteSystem notePrefabA;
        [SerializeField] private LongNoteSystem longNotePrefab;
        [SerializeField] private LongNoteSystem longNotePrefabA;
        [SerializeField] private LongNoteSystem bumperNotePrefab;
        [SerializeField] private LongNoteSystem bumperNotePrefabA;

        [Header("Debugging")] 
        
        [SerializeField] private SkinSystem skinPrefab;
        [SerializeField] private SkinSystem skinPrefab5K;
        [SerializeField] private SkinSystem skinPrefab6K;
        [SerializeField] private SkinSystem skinPrefab8K;
        [SerializeField] private Transform playZone;

        private GameManager gm;
        private List<Queue<NoteSystem>> noteQueues;
        private List<NoteState> noteStates;
        private SkinSystem skin;
        private Transform noteContainer;
        private int line;

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
            inputReader.PlayKeyEvent += JudgePlayKey;
            inputReader.PlayKeyDownEvent += JudgePlayKeyDown;
            inputReader.PlayKeyUpEvent += JudgePlayKeyUp;
            inputReader.PlayKeyInterruptEvent += JudgePlayKeyUp;
            player.GamePauseEvent += OnPause;
            player.GameResumeEvent += OnResume;
        }

        private void OnDisable()
        {
            inputReader.PlayKeyEvent -= JudgePlayKey;
            inputReader.PlayKeyDownEvent -= JudgePlayKeyDown;
            inputReader.PlayKeyUpEvent -= JudgePlayKeyUp;
            inputReader.PlayKeyInterruptEvent -= JudgePlayKeyUp;
            player.GamePauseEvent -= OnPause;
            player.GameResumeEvent -= OnResume;
        }

        public void PrepareNotes(SerializableDesc desc, SerializablePattern pattern)
        {
            skin = pattern.line switch
            {
                4 => skinPrefab,
                5 => skinPrefab5K,
                6 => skinPrefab6K,
                8 => skinPrefab8K,
                _ => null
            };
            line = pattern.line;
            skin = Instantiate(skin, playZone);
            noteContainer = skin.noteContainer;

            player.EndTime = 3f;
            var sortReady = new List<List<NoteSystem>>();
            for (var i = 0; i < pattern.line; i++)
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
            var target = line switch
            {
                4 => item.line == 0 || item.line == 3 ? notePrefab : notePrefabA,
                5 => item.line == 1 || item.line == 3 ? notePrefabA : notePrefab,
                6 => item.line == 2 || item.line == 4 ? notePrefabA : notePrefab,
                8 => item.line == 2 || item.line == 4 ? notePrefabA : notePrefab,
                _ => null
            };
            var noteSystem = Instantiate(target, noteContainer);
            noteSystem.SetFromData(item);

            noteSystem.transform.localPosition = new Vector3(skin.xPositions[item.line], 0);
            noteSystem.time = player.Meta.GetTime(item.beat);
            noteSystem.transform.localScale = new Vector3(skin.scale, skin.scale, 1);

            return noteSystem;
        }

        private LongNoteSystem CreateLongNote(SerializableLongNote item)
        {
            var target = line switch
            {
                4 => ((item.line == 0 || item.line == 3) ? longNotePrefab : longNotePrefabA),
                5 => ((item.line == 1 || item.line == 3) ? longNotePrefabA : longNotePrefab),
                6 => ((item.line == 1 || item.line == 4) ? longNotePrefabA : longNotePrefab),
                8 => ((item.line == 1 || item.line == 4) ? longNotePrefabA : longNotePrefab),
                _ => null
            };
            var longNoteSystem = Instantiate(target, noteContainer);
            longNoteSystem.SetFromData(item);

            longNoteSystem.time = player.Meta.GetTime(item.beat);
            longNoteSystem.endTime = player.Meta.GetTime(item.beat + item.length);
            longNoteSystem.transform.localPosition = new Vector3(skin.xPositions[item.line], 0);
            longNoteSystem.SetScale(skin.scale);

            return longNoteSystem;
        }

        private void RemoveBreakNotes()
        {
            for (var i = 0; i < noteQueues.Count; i++)
            {
                if (noteStates[i].isInLongNote) continue;

                if (noteQueues[i].Count == 0) continue;

                var gap = noteStates[i].pausedWhileIsIn
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