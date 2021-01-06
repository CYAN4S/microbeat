using UnityEngine;

public enum JUDGES { PRECISE, GREAT, NICE, BAD, BREAK };
public enum NOTECODE { NOTE, LONGNOTE };

public enum SHEETVER { A1, A3 };

public static class CONST
{
    public static readonly double[] DELTASPEED = { -0.1, -0.5, 0.5, 0.1 };
    public static readonly float[] JUDGESTD = { 0.05f, 0.1f, 0.2f, 0.3f };
    public static readonly int[] JUDGESCORE = { 5, 3, 2, 1 };

    public static readonly float[] LINEXPOS = { -300, -100, 100, 300 };

    public static readonly string[] PATTERN = { "NM", "HD", "MX", "SC" };

    public static readonly int[] RANK = { 295000, 290000, 275000, 250000, 200000 };
    public static readonly string[] RANKNAME = { "S", "A", "B", "C", "D", "F" };
}