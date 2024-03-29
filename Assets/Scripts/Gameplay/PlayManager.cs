﻿using System;
using System.Collections.Generic;
using Core;
using Core.SO.NormalChannel;
using Gameplay.Visual;
using Input;
using SO;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(NoteFactory), typeof(GameManager))]
    public class PlayManager : MonoBehaviour
    {
        [Header("Requirement")]
        [SerializeField] private PlayerSO player;
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Transform playZone;
        [SerializeField] private SkinCollection skins;
        
        [Header("Channel to get values from previous scene")]
        [SerializeField] private IntEventChannelSO gear;
        
        private GameManager gm;
        private NoteFactory factory;
        private List<Queue<NoteSystem>> noteQueues;
        private List<NoteState> noteStates;
        private SkinSystem skin;
        private int line;

        private float stdPausedTime = -100f;

        private void Awake()
        {
            noteStates = new List<NoteState>();
            gm = GetComponent<GameManager>();
            factory = GetComponent<NoteFactory>();
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

        public void Prepare(SerializableDesc desc, SerializablePattern pattern)
        {
            var skinPrefab = pattern.line switch
            {
                4 => skins.gearSet[gear.value].gear4K,
                5 => skins.gearSet[gear.value].gear5K,
                6 => skins.gearSet[gear.value].gear6K,
                8 => skins.gearSet[gear.value].gear8K,
                _ => null
            };
            line = pattern.line;
            skin = Instantiate(skinPrefab, playZone);

            noteQueues = factory.PrepareNotes(pattern, skin);
            for (var i = 0; i < pattern.line; i++)
            {
                noteStates.Add(new NoteState());
            }
        }

        private void RemoveBreakNotes()
        {
            if (!player.State.IsPlayable) return;
            
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

        private void RemoveLongNoteLater(float time)
        {
            
        }

        private void JudgePlayKeyDown(int key)
        {
            if (!player.State.IsPlayable) return;
            
            if (noteQueues[key].Count == 0) return;
            var peek = noteQueues[key].Peek();
            
            var gap = peek.time - player.CurrentTime; // HOW FAST
            if (noteStates[key].pausedWhileIsIn)
                gap = noteStates[key].target.pausedTime - player.CurrentTime;

            if (gap > Const.JUDGE_STD[(int) Judges.Bad]) // DONT CARE
                return;

            if (player.State.IsCountingToResume)
                gap = Math.Abs(gap) > Math.Abs(peek.time - stdPausedTime) ? gap : peek.time - stdPausedTime;
            
            if (peek.CompareTag("Note"))
            {
                HandleNote(key, gap);
                return;
            }

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
                    HandleCutOffLongNoteDown(key, gap);
                }
                return;
            }

            Debug.LogError("Unknown Tag: " + peek.tag);
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
            stdPausedTime = Math.Max(player.CurrentTime, stdPausedTime);
            
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