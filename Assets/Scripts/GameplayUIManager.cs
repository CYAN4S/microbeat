using System;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    public static readonly Color[] detailColor = {new Color(0, 222f / 256f, 1), new Color(1, 171f / 256f, 0)};
    public static readonly string[] judgeTriggers = {"Precise", "Great", "Nice", "Bad", "Break"};
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private PlayerSO player;

    public Text speedText, scoreText;
    public Text timeText, beatText, bpmText;
    public Text detailText;
    public TextMeshProUGUI comboTMP;

    public Animator judgeAnimator;
    public Animator grooveLight;
    public Animator comboAnimator;
    public Animator comboHeadingAnimator;
    public Animator[] noteEffects;

    private void OnEnable()
    {
        player.BpmChangeEvent += ChangeBpmText;
        player.ScrollSpeedChangeEvent += ChangeSpeedText;
    }

    private void OnDisable()
    {
        player.BpmChangeEvent -= ChangeBpmText;
        player.ScrollSpeedChangeEvent -= ChangeSpeedText;
    }

    private void Start()
    {
        ChangeBpmText();
        ChangeSpeedText();
    }

    private void LateUpdate()
    {
        if (!player.IsWorking) return;

        grooveLight.SetFloat("BPM", (float)player.CurrentBpm / 60f);
        timeText.text = player.CurrentTime.ToString("F3");
        beatText.text = player.CurrentBeat.ToString("F0");
    }
    
    private void ChangeBpmText() =>  bpmText.text = player.CurrentBpm.ToString("F1") + " BPM";
    private void ChangeSpeedText() => speedText.text = "X" + player.ScrollSpeed.ToString("F1");

    public void VisualizeJudge(JUDGES judge)
    {
        TriggerJudgeAnimation(judge);
    }

    public void VisualizeNoteEffect(int line)
    {
        noteEffects[line].SetTrigger("Effect");
    }

    public void TriggerJudgeAnimation(JUDGES judge)
    {
        judgeAnimator.SetTrigger(judgeTriggers[(int) judge]);
    }

    public void ShowGap(float gap)
    {
        detailText.color = detailColor[gap > 0 ? 1 : 0];
        detailText.text = (gap > 0 ? "EARLY " : "LATE ") + (Math.Abs(gap) * 100).ToString("F0");
    }

    public void EraseGap()
    {
        detailText.text = "";
    }

    public void ShowScore(double score)
    {
        scoreText.text = score.ToString("F0");
    }

    public void StartGroove()
    {
        grooveLight.SetTrigger("Begin");
    }

    public void StopGroove()
    {
        grooveLight.SetTrigger("End");
    }

    public void ShowCombo(int combo)
    {
        comboTMP.text = combo.ToString();
        comboAnimator.SetTrigger("Combo");
        comboHeadingAnimator.SetTrigger("ComboHeading");
    }
}