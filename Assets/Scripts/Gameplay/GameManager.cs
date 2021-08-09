using System;
using System.Collections;
using Core;
using Input;
using SO;
using SO.NormalChannel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [Header("Requirement")]
        [SerializeField] private PlayerSO player;
        [SerializeField] private InputReader inputReader;

        [Header("Channel to get values from previous scene")] 
        [SerializeField] private ChartEventChannelSO onChartSelect;

        private AudioSource audioSource;

        private int currentMetaIndex;
        private int noteCount;

        private PlayManager pm;
        private int rawScore;

        private void Awake()
        {
            pm = GetComponent<PlayManager>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            player.Reset();
            currentMetaIndex = 0;
            rawScore = 0;

            var chart = onChartSelect.value;

            if (chart == null)
            {
                Debug.LogError("No ChartPath in channel.");
                return;
            }

            var desc = chart.descriptionData;
            var pattern = chart.patternData;

            noteCount = pattern.notes.Count + pattern.longNotes.Count;
            var meta = new BpmMeta(desc.bpms, desc.stdBpm);
            player.Meta = meta;
            AdjustTime();

            pm.Prepare(desc, pattern);

            audioSource.clip = chart.audioClip;
            if (audioSource.clip != null) StartCoroutine(PlayAudio(0));
            StartGame();
        }

        private void Update()
        {
            if (player.State.IsTimePassing)
                AdjustTime();
        }

        private void OnEnable()
        {
            inputReader.PauseKeyEvent += PauseOrResume;
        }

        private void OnDisable()
        {
            inputReader.PauseKeyEvent -= PauseOrResume;
        }

        private void StartGame()
        {
            player.OnGameStart();
            inputReader.SpeedEvent += ChangeSpeed;
        }

        private void EndGame()
        {
            player.OnGameEnd();
            SceneManager.LoadScene(3);
        }

        private void AdjustTime()
        {
            if (player.CurrentTime >= player.EndTime)
            {
                EndGame();
                return;
            }

            player.CurrentTime += Time.deltaTime;

            RefreshBeatAndBpm();
        }

        private void RefreshBeatAndBpm()
        {
            if (player.Meta.endTimes[currentMetaIndex] <= player.CurrentTime) currentMetaIndex++;

            if (currentMetaIndex == 0)
            {
                player.CurrentBeat = player.CurrentTime * player.Meta.bpms[0] / 60.0;
                player.ChangeBpm(player.Meta.bpms[0]);
                return;
            }

            player.CurrentBeat = player.Meta.beats[currentMetaIndex] +
                                 (player.CurrentTime - player.Meta.endTimes[currentMetaIndex - 1]) *
                                 player.Meta.bpms[currentMetaIndex] / 60.0;
            player.ChangeBpm(player.Meta.bpms[currentMetaIndex]);
        }

        public IEnumerator PlayAudio(float time)
        {
            while (time > player.CurrentTime) yield return null;
            audioSource.Play();
            player.OnZero();
        }

        private void ChangeSpeed(int input)
        {
            var value = player.ScrollSpeed + Const.DELTA_SPEED[input];

            if (value < 0.5)
                value = 0.5;
            else if (value > 9.5) value = 9.5;

            player.ChangeScrollSpeed(value);
        }

        public void PauseOrResume()
        {
            if (player.State.IsPausable)
                Pause();
            else if (player.State.IsPaused) Resume();
        }

        private void Pause()
        {
            player.OnGamePause();
            audioSource.Pause();
        }

        private void Resume()
        {
            var currentTime = player.CurrentTime;
            var act = player.OnGameResume(Const.WAIT_TIME);
            StartCoroutine(InvokeAt(act, currentTime));
            // StartCoroutine(WaitAndStart(act, Const.WAIT_TIME));

            if (player.CurrentTime < 0)
            {
                audioSource.time = 0;
                StartCoroutine(PlayAudio(0));
            }
            else
            {
                audioSource.time = player.CurrentTime;
                audioSource.UnPause();
            }
        }

        private IEnumerator WaitAndStart(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            action();
        }

        private IEnumerator InvokeAt(Action action, float time)
        {
            while (time > player.CurrentTime) yield return null;
            action?.Invoke();
        }

        public void ApplyNote(int line, Judges judge, float gap)
        {
            player.JudgeCounts[(int) judge]++;

            rawScore += Const.JUDGE_SCORE[(int) judge];
            player.ChangeScore((double) rawScore / (Const.JUDGE_SCORE[0] * noteCount) * 300000d);

            if (judge != Judges.Bad)
            {
                player.IncreaseCombo(1);
                player.OnNoteEffect(line);
            }
            else
            {
                player.BreakCombo();
            }

            player.OnJudge(judge);
            player.OnGap(gap);
        }

        public void ApplyBreak(int line)
        {
            player.JudgeCounts[(int) Judges.Break]++;
            player.BreakCombo();
            player.OnJudge(Judges.Break);
        }

        public void ApplyLongNoteStart(int line, Judges judge, float gap)
        {
            if (judge == Judges.Bad)
            {
                player.BreakCombo();
            }
            else
            {
                player.IncreaseCombo(1);
                player.OnNoteEffect(line);
            }

            player.OnGap(gap);
            player.OnJudge(judge);
        }

        public void ApplyCutOffLongNoteStart(int line, Judges judge)
        {
            player.OnNoteEffect(line);
            player.OnJudge(judge);
        }

        public void ApplyLongNoteTick(int line, Judges judge)
        {
            player.IncreaseCombo(1);
            player.OnJudge(judge);
            player.OnNoteEffect(line);
        }

        public void ApplyLongNoteEnd(int line, Judges judge, float gap)
        {
            ApplyNote(line, judge, gap);
        }
    }
}