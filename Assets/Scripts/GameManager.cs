﻿using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private AudioSource audioSource;
    private InputManager im;
    private FileExplorer fe;
    private UIManager ui;
    private PlayManager pm;
    private IngameGraphicsManager igm;

    public static bool IsWorking { get; private set; }
    public static float CurrentTime { get; private set; }
    public static double ScrollSpeed { get; private set; }
    public static float EndTime { get; set; }
    public static int Combo { get; private set; }
    public static double Score { get; private set; }
    public static double CurrentBeat { get; private set; }
    public static double CurrentBpm { get; private set; }
    public int[] JudgeCounts { get; private set; } = { 0, 0, 0, 0, 0 };

    public Action<SheetData> OnSheetSelect;
    public Action OnScrollSpeedChange;

    public BpmMeta Meta { get; private set; }

    private int rawScore;
    private int noteCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        im = GetComponent<InputManager>();
        fe = GetComponent<FileExplorer>();
        ui = GetComponent<UIManager>();
        pm = GetComponent<PlayManager>();
        igm = GetComponent<IngameGraphicsManager>();

        OnSheetSelect += _ => PrepareGame(_);
    }


    private void Start()
    {
        InitVariables();
        PrepareSongList();
    }


    private void Update()
    {
        if (!IsWorking)
        {
            return;
        }

        RefreshTime();
    }

    private void PrepareGame(SheetData sheetData)
    {
        ui.selection.SetActive(false);

        SerializableDesc desc = sheetData.desc;
        SerializableSheet sheet = sheetData.sheet;

        noteCount = sheet.notes.Count + sheet.longNotes.Count;
        Meta = desc.bpms?.Count is int c && c != 0 ? new BpmMeta(desc.bpms, desc.bpm) : new BpmMeta(desc.bpm);

        pm.PrepareNotes(desc, sheet);

        StartCoroutine
        (
            fe.GetAudioClip(sheetData.audioPath, () =>
            {
                audioSource.clip = fe.streamAudio;
                if (audioSource.clip != null)
                {
                    StartCoroutine(PlayAudio(0));
                }

                pm.SetPlayKey();
                StartGame();
            })
        );
    }

    private void StartGame()
    {
        IsWorking = true;
        im.OnSpeedKeyDown += ChangeSpeed;
    }

    private void EndGame()
    {
        igm.StopGroove();
        ui.DisplayResult();
    }

    private void InitVariables()
    {
        IsWorking = false;
        CurrentTime = -3;
        ScrollSpeed = 2.5;
        EndTime = 1000f;
        Combo = 0;
        Score = 0;
        CurrentBeat = 0;
        CurrentBpm = 0;
        currentMetaIndex = 0;
        rawScore = 0;
    }

    private void PrepareSongList()
    {
        StartCoroutine(fe.ExploreAsync(GetComponent<UIManager>().DisplayMusics));
    }

    private void RefreshTime()
    {
        if (CurrentTime >= EndTime)
        {
            EndGame();
            IsWorking = false;
            return;
        }

        CurrentTime += Time.deltaTime;

        RefreshBeatAndBpm();
    }

    private int currentMetaIndex = 0;
    private void RefreshBeatAndBpm()
    {
        if (Meta.endTimes[currentMetaIndex] <= CurrentTime)
        {
            currentMetaIndex++;
        }

        if (currentMetaIndex == 0)
        {
            CurrentBeat = CurrentTime * Meta.bpms[0] / 60.0;
            CurrentBpm = Meta.bpms[0];
            return;
        }

        CurrentBeat = Meta.beats[currentMetaIndex] + (CurrentTime - Meta.endTimes[currentMetaIndex - 1]) * Meta.bpms[currentMetaIndex] / 60.0;
        CurrentBpm = Meta.bpms[currentMetaIndex];
    }

    public IEnumerator PlayAudio(float time)
    {
        while (time > CurrentTime)
        {
            yield return null;
        }
        audioSource.Play();
        igm.StartGroove();
    }

    private void ChangeSpeed(int input)
    {
        double value = ScrollSpeed + CONST.DELTASPEED[input];

        if (value < 0.5)
        {
            value = 0.5;
        }
        else if (value > 9.5)
        {
            value = 9.5;
        }

        ScrollSpeed = value;
        OnScrollSpeedChange?.Invoke();
    }

    public void ApplyNote(int line, JUDGES judge, float gap)
    {
        JudgeCounts[(int)judge]++;

        rawScore += CONST.JUDGESCORE[(int)judge];
        Score = (double)rawScore / (CONST.JUDGESCORE[0] * noteCount) * 300000d;

        if (judge != JUDGES.BAD)
        {
            Combo++;
            igm.ShowCombo(Combo);
            igm.VisualizeNoteEffect(line);
        }
        else
        {
            Combo = 0;
        }

        igm.ShowGap(gap);
        igm.ShowScore(Score);
        igm.VisualizeJudge(judge);
    }

    public void ApplyBreak(int line)
    {
        JudgeCounts[(int)JUDGES.BREAK]++;
        Combo = 0;

        igm.EraseGap();
        igm.VisualizeJudge(JUDGES.BREAK);
    }

    public void ApplyLongNoteStart(int line, JUDGES judge, float gap)
    {
        if (judge == JUDGES.BAD)
        {
            Combo = 0;
        }
        else
        {
            Combo++;
            igm.ShowCombo(Combo);
            igm.VisualizeNoteEffect(line);
        }

        igm.ShowGap(gap);
        igm.VisualizeJudge(judge);
    }

    public void ApplyLongNoteTick(int line, JUDGES judge)
    {
        Combo++;
        igm.ShowCombo(Combo);
        igm.VisualizeJudge(judge);
        igm.VisualizeNoteEffect(line);
    }

    public void ApplyLongNoteEnd(int line, JUDGES judge, float gap)
    {
        ApplyNote(line, judge, gap);
    }




}
