using System;

namespace Core
{
    public enum Judges
    {
        Precise,
        Great,
        Nice,
        Bad,
        Break
    }

    public static class Const
    {
        public static readonly double[] DELTA_SPEED = {-0.1, -0.5, 0.5, 0.1};

        public static readonly float[] JUDGE_STD = {0.05f, 0.1f, 0.2f, 0.3f};
        public static readonly int[] JUDGE_SCORE = {5, 3, 2, 1};
        public static readonly string[] JUDGE_NAME = {"Precise", "Great", "Nice", "Bad", "Break"};

        public static readonly string[] PATTERN = {"SNT", "ORG", "ADV", "HYP", "EXC"};

        public static readonly int[] RANK = {295000, 290000, 275000, 250000, 200000};
        public static readonly string[] RANK_NAME = {"S", "A", "B", "C", "D", "F"};

        public static readonly float WAIT_TIME = 5f;
    }

    [Serializable]
    public enum GameState
    {
        Title = 0,
        MusicSelect = 1,
        Gameplay = 2,
        Result = 3
    }
}