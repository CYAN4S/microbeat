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
    public static readonly double[] DELTASPEED = { -0.1, -0.5, 0.5, 0.1 };
    public static readonly float[] JUDGESTD = { 0.05f, 0.1f, 0.2f, 0.3f };
    public static readonly int[] JUDGESCORE = { 5, 3, 2, 1 };

    public static readonly KeyCode[] PLAYKEYCODES = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };
    public static readonly KeyCode[] SPEEDKEYCODES = { KeyCode.E, KeyCode.R, KeyCode.U, KeyCode.I };

    public static readonly float[] LINEXPOS = { -300, -100, 100, 300 };

    public static readonly string[] PATTERN = { "NM", "HD", "MX", "SC" };

    public static readonly int[] RANK = { 295000, 290000, 275000, 250000, 200000 };
    public static readonly string[] RANKNAME = { "S", "A", "B", "C", "D", "F" };
}