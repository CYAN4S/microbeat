using Core;
using UnityEngine;
using UnityEngine.Events;

public enum PlayState
{
    Loading, Playable, Paused, ResumeCount
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
        // public double StdBpm { get; set; }
        public BpmMeta Meta { get; set; }

        // Status
        public bool IsWorking { get; private set; }
        // public bool IsPaused { get; private set; }
        
        public PlayState State { get; private set; }
        
        public double ScrollSpeed { get; private set; }
        public double CurrentBpm { get; private set; }
        public int Combo { get; private set; }
        public double Score { get; private set; }

        public float GrooveMeter { get; private set; }
        public int[] JudgeCounts { get; private set; }


        public void Reset()
        {
            IsWorking = false;
            // IsPaused = false;
            State = PlayState.Loading;
            CurrentTime = -3;
            ScrollSpeed = 2.5;
            EndTime = 1000f;
            Combo = 0;
            Score = 0;
            CurrentBeat = 0;
            CurrentBpm = 0;
            JudgeCounts = new int[5];
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
            State = PlayState.Playable;
            GameStartEvent?.Invoke();
        }

        public void OnGameEnd()
        {
            IsWorking = false;
            GameEndEvent?.Invoke();
        }

        public void OnGamePause()
        {
            State = PlayState.Paused;
            GamePauseEvent?.Invoke();
        }

        public void OnGameResume()
        {
            State = PlayState.ResumeCount;
            CurrentTime = Mathf.Max(-3f, CurrentTime - 3);
            GameResumeEvent?.Invoke();
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

        public void SetStatePlayable()
        {
            State = PlayState.Playable;
        }
    }
}