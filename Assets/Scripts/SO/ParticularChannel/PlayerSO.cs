using System;
using Core;
using UnityEngine;
using UnityEngine.Events;

public enum PlayState
{
    Loading,
    Playing,
    Paused,
    ResumeCount // Playable, non-pausable
}

[Serializable]
public class GameplayState
{
    public PlayState value = PlayState.Loading;

    public bool IsLoading => (value == PlayState.Loading);

    public bool IsPlayable => value == PlayState.Playing || value == PlayState.ResumeCount;

    // Time passes only this is true.
    public bool IsTimePassing => IsPlayable;

    public bool IsPaused => value == PlayState.Paused;
    public bool IsCountingToResume => value == PlayState.ResumeCount;

    public bool IsPausable => value == PlayState.Playing;
}

namespace SO
{
    [CreateAssetMenu(menuName = "Player")]
    public class PlayerSO : ScriptableObject
    {
        // Update
        public float CurrentTime { get; set; }
        public double CurrentBeat { get; set; }

        // Constant of chart
        public float EndTime { get; set; }
        public BpmMeta Meta { get; set; }

        // Status
        public bool IsWorking { get; private set; }
        public GameplayState State { get; private set; }

        public double ScrollSpeed { get; private set; }
        public double CurrentBpm { get; private set; }
        public int Combo { get; private set; }
        public double Score { get; private set; }

        public float GrooveMeter { get; private set; }
        public float FeverMeter { get; private set; }
        public int[] JudgeCounts { get; private set; }


        public void Reset()
        {
            IsWorking = false;
            State = new GameplayState();
            CurrentTime = -5;
            ScrollSpeed = 2.5;
            EndTime = 1000f;
            Combo = 0;
            Score = 0;
            CurrentBeat = 0;
            CurrentBpm = 0;
            JudgeCounts = new int[Const.JUDGE_NAME.Length];
        }

        public event UnityAction BpmChangeEvent;
        public event UnityAction ScrollSpeedChangeEvent;
        public event UnityAction ScoreChangeEvent;
        public event UnityAction GameStartEvent;
        public event UnityAction GameEndEvent;
        public event UnityAction ComboIncreaseEvent;
        public event UnityAction ComboBreakEvent;
        public event UnityAction GamePauseEvent;
        public event UnityAction GameResumeEvent;
        public event UnityAction CountOverEvent;

        public event UnityAction<Judges> JudgeEvent;
        public event UnityAction<int> NoteEffectEvent;
        public event UnityAction ZeroEvent;
        public event UnityAction<float> GapEvent;

        public void ChangeBpm(double value)
        {
            CurrentBpm = value;
            BpmChangeEvent?.Invoke();
        }

        public void ChangeScrollSpeed(double value)
        {
            ScrollSpeed = value;
            ScrollSpeedChangeEvent?.Invoke();
        }

        public void ChangeScore(double value)
        {
            Score = value;
            ScoreChangeEvent?.Invoke();
        }

        public void OnGameStart()
        {
            IsWorking = true;
            State.value = PlayState.Playing;
            GameStartEvent?.Invoke();
        }

        public void OnGameEnd()
        {
            IsWorking = false;
            GameEndEvent?.Invoke();
        }

        public void OnGamePause()
        {
            State.value = PlayState.Paused;
            GamePauseEvent?.Invoke();
        }

        public Action OnGameResume(float backTime)
        {
            State.value = PlayState.ResumeCount;
            CurrentTime -= backTime;
            GameResumeEvent?.Invoke();
            return OnCountOver;
        }

        private void OnCountOver()
        {
            State.value = PlayState.Playing;
            CountOverEvent?.Invoke();
        }

        public void IncreaseCombo(int delta)
        {
            Combo += delta;
            ComboIncreaseEvent?.Invoke();
        }

        public void BreakCombo()
        {
            Combo = 0;
            ComboBreakEvent?.Invoke();
        }

        public void OnJudge(Judges value)
        {
            JudgeEvent?.Invoke(value);
        }

        public void OnNoteEffect(int line)
        {
            NoteEffectEvent?.Invoke(line);
        }

        public void OnZero()
        {
            ZeroEvent?.Invoke();
        }

        public void OnGap(float value)
        {
            GapEvent?.Invoke(value);
        }
    }
}