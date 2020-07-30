using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VFX;

public enum JUDGES { PRECISE, GREAT, NICE, BAD, BREAK };

public static class CONST
{
    public static readonly double[] DELTASPEED = { -0.05, -0.1, -0.5, 0.5, 0.1, 0.05 };
    public static readonly float[] JUDGESTD = { 0.05f, 0.1f, 0.2f, 0.3f };
    public static readonly int[] JUDGESCORE = { 5, 3, 2, 1 };

    public static readonly KeyCode[] PLAYKEYCODES = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };
    public static readonly KeyCode[] SPEEDKEYCODES = { KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I };

    public static readonly float[] LINEXPOS = { -300, -100, 100, 300 };

    public static readonly string[] PATTERN = { "NM", "HD", "MX", "SC" };
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static bool IsWorking { get; private set; }
    public static float CurrentTime { get; private set; }
    public static double ScrollSpeed { get; private set; }
    public static float EndTime { get; set; }
    public static int Combo { get; private set; }
    public static double Score { get; private set; }

    public Action OnGameStart;
    public Action OnMusicStart;
    public Action OnScrollSpeedChange;
    public Action<JUDGES, float> OnJudge;
    public Action OnGameEnd;

    public AudioClip AudioClip { get; set; }

    public Sheet CurrentSheet { get; private set; }

    private int dataScore;

    private AudioSource audioSource;

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
            NongameUIManager.Instance.DisplayResult();
            OnGameEnd();

            IsWorking = false;
            return;
        }

        CurrentTime += Time.deltaTime;

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

    public void StartMusic()
    {
        audioSource.clip = AudioClip;
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
        OnMusicStart?.Invoke();
    }

    public IEnumerator StartMusicOnTime(float time)
    {
        while (time > CurrentTime)
        {
            yield return null;
        }
        audioSource.clip = AudioClip;
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    public void SheetSelect(DirectoryInfo dir, SerializableDesc desc, SerializableSheet sheet)
    {
        string audioPath = Path.Combine(dir.FullName, desc.musicPath);

        StartCoroutine(GetComponent<FileExplorer>().GetAudioClip(audioPath, () =>
        {
            CurrentSheet = new Sheet(desc, sheet, audioSource.clip);

            OnGameStart?.Invoke();
            StartCoroutine(StartMusicOnTime(0));
        }));
    }

    public void ActOnJudge(JUDGES judge, float gap)
    {
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

        OnJudge(judge, gap);
    }

}
