using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    [CreateAssetMenu(menuName = "Player")]
    public class PlayerSO : ScriptableObject
    {
        // Update
        public float CurrentTime { get; set; }
        public double CurrentBeat { get; set; }

        public float EndTime { get; set; }
        public double StdBpm { get; set; }
        public BpmMeta Meta { get; set; }

        public double CurrentBpm { get; private set; }
        public bool IsWorking { get; private set; }
        public double ScrollSpeed { get; private set; }

        public int Combo { get; private set; }
        public double Score { get; private set; }

        public float GrooveMeter { get; private set; }
        public int[] JudgeCounts { get; private set; }


        public void Reset()
        {
            IsWorking = false;
            CurrentTime = -3;
            ScrollSpeed = 2.5;
            EndTime = 1000f;
            Combo = 0;
            Score = 0;
            CurrentBeat = 0;
            CurrentBpm = 0;
            JudgeCounts = new int[5];
            ResetEvent?.Invoke();
        }

        public event UnityAction ResetEvent;
        public event UnityAction BpmChangeEvent;
        public event UnityAction ScrollSpeedChangeEvent;
        public event UnityAction ScoreChangeEvent;
        public event UnityAction GameStartEvent;
        public event UnityAction GameEndEvent;
        public event UnityAction ComboIncreaseEvent;
        public event UnityAction ComboBreakEvent;

        public event UnityAction<JUDGES> JudgeEvent;
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
            GameStartEvent?.Invoke();
        }

        public void OnGameEnd()
        {
            IsWorking = false;
            GameEndEvent?.Invoke();
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

        public void OnJudge(JUDGES value)
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