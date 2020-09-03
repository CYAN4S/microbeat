using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private AudioSource audioSource;

    public static bool IsWorking { get; private set; }
    public static float CurrentTime { get; private set; }
    public static double ScrollSpeed { get; private set; }
    public static float EndTime { get; set; }
    public static int Combo { get; private set; }
    public static double Score { get; private set; }

    public Action OnSheetSelect;
    public Action OnGameStart;
    public Action OnMusicStart;

    public Action OnScrollSpeedChange;
    public Action<int, JUDGES, float> OnJudge;

    public Action OnGameEnd;

    public AudioClip AudioClip { get; set; }

    public Sheet CurrentSheet { get; private set; }
    public BpmMeta CurrentBpm { get; private set; }

    private int dataScore;
    public int[] JudgeCounts { get; private set; } = { 0, 0, 0, 0, 0 };


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();

        OnGameStart += () =>
        {
            IsWorking = true;
            InputManager.Instance.OnSpeedKeyDown += ChangeSpeed;
        };

        OnMusicStart += () =>
        {

        };
    }

    private void Start()
    {
        IsWorking = false;
        CurrentTime = -3;
        ScrollSpeed = 2.5;
        EndTime = 1000f;
        Combo = 0;
        Score = 0;

        dataScore = 0;
    }

    private void Update()
    {
        if (!IsWorking)
        {
            return;
        }

        if (CurrentTime >= EndTime)
        {
            OnGameEnd();

            IsWorking = false;
            return;
        }

        CurrentTime += Time.deltaTime;
    }

    public void SheetSelect(SerializableDesc desc, SerializableSheet sheet, string audioPath)
    {
        OnSheetSelect?.Invoke();

        CurrentSheet = new Sheet(desc, sheet);
        if (CurrentSheet.bpms?.Count is int x && x != 0)
        {
            CurrentBpm = new BpmMeta(CurrentSheet.bpms, CurrentSheet.endBeat);
        }
        
        StartCoroutine(GetComponent<FileExplorer>().GetAudioClip(audioPath, () =>
        {
            StartCoroutine(StartMusicOnTime(0));
            OnGameStart?.Invoke();
        }));
    }

    public IEnumerator StartMusicOnTime(float time)
    {
        while (time > CurrentTime)
        {
            yield return null;
        }
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
        OnMusicStart?.Invoke();
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

    public void HandleJudge(int line, JUDGES judge, float gap)
    {
        JudgeCounts[(int)judge]++;

        if (judge == JUDGES.BREAK)
        {
            Combo = 0;
        }
        else
        {
            dataScore += CONST.JUDGESCORE[(int)judge];
            Score = (double)dataScore / (CONST.JUDGESCORE[0] * CurrentSheet.notes.Count) * 300000d;
            Combo = (judge != JUDGES.BAD) ? (Combo + 1) : 0;
        }

        OnJudge(line, judge, gap);
    }
}
