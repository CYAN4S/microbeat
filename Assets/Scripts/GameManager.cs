using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static bool IsWorking { get; private set; }
    public static Action OnCallSheet = () => IsWorking = true;

    public static float CurrentTime { get; private set; }

    public static double ScrollSpeed { get; private set; }
    public static Action OnScrollSpeedChange = () => { };
    public static readonly double[] DELTASPEED = { -0.05, -0.1, -0.5, 0.5, 0.1, 0.05 };

    public static Sheet CurrentSheet { get; private set; }
    public Sheet SheetInInspector;

    public static readonly float[] JUDGESTD = { 0.05f, 0.1f, 0.3f, 0.5f };

    private void Awake()
    {
        instance = this;
        IsWorking = false;

        OnCallSheet += () => InputManager.OnSpeedKeyDown += ChangeSpeed;
    }

    private void Start()
    {
        CurrentTime = -5;
        ScrollSpeed = 2.5;

        CurrentSheet = SheetInInspector;
    }

    private void Update()
    {
        if (IsWorking)
        {
            CurrentTime += Time.deltaTime;
        }
    }

    private void ChangeSpeed(int input)
    {
        double value = ScrollSpeed + DELTASPEED[input];
        if (value < 0.5 || value > 9.5)
        {
            return;
        }

        ScrollSpeed = value;
        OnScrollSpeedChange();
    }

    public void Launch()
    {
        OnCallSheet();

    }

}
