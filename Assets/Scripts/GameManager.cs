using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VFX;

public enum JUDGES { PRECISE, GREAT, NICE, BAD, BREAK };

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static bool IsWorking { get; private set; }
    public static Action OnCallSheet = () => IsWorking = true;

    public static float CurrentTime { get; private set; }

    public static double ScrollSpeed { get; private set; }
    public static Action OnScrollSpeedChange = () => { };
    public static readonly double[] DELTASPEED = { -0.05, -0.1, -0.5, 0.5, 0.1, 0.05 };

    public static Sheet CurrentSheet { get; private set; }
    public Sheet SheetInInspector;
    public static float EndTime { get; set; }

    public static readonly float[] JUDGESTD = { 0.05f, 0.1f, 0.3f, 0.5f };

    public UIManager ui;

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
        CurrentTime = -5;
        ScrollSpeed = 2.5;

        CurrentSheet = SheetInInspector;
    }

    private void Update()
    {
        if (!IsWorking)
        {
            return;
        }

        CurrentTime += Time.deltaTime;

        if (CurrentTime >= EndTime)
        {
            NongameUIManager.Instance.DisplayResult();
        }
    }

    private void ChangeSpeed(int input)
    {
        double value = ScrollSpeed + DELTASPEED[input];

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

    public void Launch()
    {
        OnCallSheet();
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
        }
    }

}
