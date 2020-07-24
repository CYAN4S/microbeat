using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
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
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static bool IsWorking { get; private set; }
    public static float CurrentTime { get; private set; }
    public static double ScrollSpeed { get; private set; }

    public static Action OnCallSheet = () => IsWorking = true;
    public static Action OnScrollSpeedChange = () => { };

    public static Sheet CurrentSheet { get; private set; }
    public Sheet SheetInInspector;
    public static float EndTime { get; set; }

    private int dataScore;
    private double score;

    public UIManager ui;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        IsWorking = false;
        OnCallSheet += () => InputManager.Instance.OnSpeedKeyDown += ChangeSpeed;
    }

    private void Start()
    {
        CurrentTime = -3;
        ScrollSpeed = 2.5;
        dataScore = 0;
        score = 0;

        CurrentSheet = SheetInInspector;

        audioSource = GetComponent<AudioSource>();
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
        OnScrollSpeedChange();
    }

    public void StartMusic()
    {
        audioSource.clip = CurrentSheet.audioClip;
        audioSource.Play();
    }

    public void Launch()
    {
        OnCallSheet();
        Invoke("StartMusic", 3f);
    }

    public void ActOnJudge(JUDGES judge, float gap)
    {
        ui.LaunchJudge(judge);

        if (judge == JUDGES.BREAK)
        {
            ui.EraseGap();
        }
        else
        {
            ui.ShowGap(gap);
            dataScore += CONST.JUDGESCORE[(int)judge];
            score = (double)dataScore / (CONST.JUDGESCORE[0] * CurrentSheet.notes.Count) * 300000d;
            ui.ShowScore(score);
        }
    }

}
