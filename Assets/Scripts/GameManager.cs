using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static bool IsWorking { get; private set; }
    public static Action OnCallSheet = () => IsWorking = true;

    public static double CurrentTime { get; private set; }

    public static double ScrollSpeed { get; private set; }
    public static Action OnScrollSpeedChange;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CurrentTime = 0;
        IsWorking = false;
    }

    private void Update()
    {
        if (IsWorking)
        {
            CurrentTime += Time.deltaTime;
        }
    }

    private void ChangeSpeed()
    {

    }


}
