using System;
using SO;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    public static readonly Color[] detailColor = {new Color(0, 222f / 256f, 1), new Color(1, 171f / 256f, 0)};
    [SerializeField] private PlayerSO player;

    public Text speedText, scoreText;
    public Text timeText, beatText, bpmText;
    public Text detailText;

    private void Start()
    {
        ChangeBpmText();
        ChangeSpeedText();
    }

    private void LateUpdate()
    {
        if (!player.IsWorking) return;

        timeText.text = player.CurrentTime.ToString("F3");
        beatText.text = ((int)player.CurrentBeat).ToString("F3");
    }

    private void OnEnable()
    {
        player.BpmChangeEvent += ChangeBpmText;
        player.ScrollSpeedChangeEvent += ChangeSpeedText;
        player.ScoreChangeEvent += ChangeScoreText;
        player.GapEvent += ChangeGapText;
        player.ComboBreakEvent += EraseGapText;
    }

    private void OnDisable()
    {
        player.BpmChangeEvent -= ChangeBpmText;
        player.ScrollSpeedChangeEvent -= ChangeSpeedText;
        player.ScoreChangeEvent -= ChangeScoreText;
        player.GapEvent -= ChangeGapText;
        player.ComboBreakEvent -= EraseGapText;
    }

    private void ChangeBpmText()
    {
        bpmText.text = player.CurrentBpm.ToString("F1") + " BPM";
    }

    private void ChangeSpeedText()
    {
        speedText.text = "X" + player.ScrollSpeed.ToString("F1");
    }

    private void ChangeScoreText()
    {
        scoreText.text = player.Score.ToString("F0");
    }

    private void ChangeGapText(float gap)
    {
        detailText.color = detailColor[gap > 0 ? 1 : 0];
        detailText.text = (gap > 0 ? "EARLY " : "LATE ") + (Math.Abs(gap) * 100).ToString("F0");
    }

    private void EraseGapText()
    {
        detailText.text = "";
    }
}