﻿using System.Collections;
using Core;
using FileIO;
using Input;
using SO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private ChartPathEventChannelSO onChartSelected;
        [SerializeField] private InputReader inputReader;
        [SerializeField] private PlayerSO player;
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

            var chartPath = onChartSelected.value;
            var chart = new Chart();

            if (chartPath == null)
            {
                Debug.LogError("No ChartPath in channel.");
                return;
            }

            chart.desc = FileExplorer.FromFile<SerializableDesc>(chartPath.descPath);
            chart.pattern = FileExplorer.FromFile<SerializablePattern>(chartPath.patternPath);
            var x = StartCoroutine(FileExplorer.GetAudioClip(chartPath.audioPath, value =>
            {
                chart.audioClip = value;
                PrepareGame(chart);
            }));
        }

        private void Update()
        {
            if (player.IsWorking && !player.IsPaused)
                RefreshTime();
        }

        private void OnEnable()
        {
            inputReader.pauseKeyEvent += PauseOrResume;
        }

        private void OnDisable()
        {
            inputReader.pauseKeyEvent -= PauseOrResume;
        }

        private void PrepareGame(Chart chart)
        {
            var desc = chart.desc;
            var pattern = chart.pattern;

            noteCount = pattern.notes.Count + pattern.longNotes.Count;
            var meta = desc.bpms?.Count is int c && c != 0 ? new BpmMeta(desc.bpms, desc.bpm) : new BpmMeta(desc.bpm);
            // player.StdBpm = meta.std;
            player.Meta = meta;

            pm.PrepareNotes(desc, pattern);

            audioSource.clip = chart.audioClip;
            if (audioSource.clip != null) StartCoroutine(PlayAudio(0));
            StartGame();
        }

        private void StartGame()
        {
            player.OnGameStart();
            inputReader.speedEvent += ChangeSpeed;
        }

        private void EndGame()
        {
            player.OnGameEnd();
            SceneManager.LoadScene(3);
        }


        private void RefreshTime()
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

        private void PauseOrResume()
        {
            if (player.IsPaused)
                Resume();
            else
                Pause();
        }

        private void Pause()
        {
            player.OnGamePause();
            audioSource.Pause();
        }

        private void Resume()
        {
            player.OnGameResume();
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

        public void ApplyLongNoteStartPausedWhileIsIn(int line, Judges judge)
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