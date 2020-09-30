using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameGraphicsManager : MonoBehaviour
{
    public GameObject[] pressEffectObjects;
    public GameObject[] pressButtonObjects;

    public Text speedText, scoreText;
    public Text timeText, beatText, bpmText;
    public Text detailText;
    public Text ComboText;

    public Animator judgeAnimator;
    public Animator grooveLight;
    public Animator comboAnimator;
    public Animator comboHeadingAnimator;
    public Animator[] noteEffects;

    public static readonly Color[] detailColor = { new Color(0, 222f / 256f, 1), new Color(1, 171f / 256f, 0) };
    public static readonly string[] judgeTriggers = { "Precise", "Great", "Nice", "Bad", "Break" };

    private void Start()
    {
        InputManager.Instance.OnPlayKeyDown += n =>
        {
            pressEffectObjects[n].SetActive(true);
            pressButtonObjects[n].SetActive(true);
        };

        InputManager.Instance.OnPlayKeyUp += n =>
        {
            pressEffectObjects[n].SetActive(false);
            pressButtonObjects[n].SetActive(false);
        };
    }

    private void LateUpdate()
    {
        if (!GameManager.IsWorking)
        {
            return;
        }

        grooveLight.SetFloat("BPM", (float)GameManager.CurrentBpm / 60f);
        speedText.text = "X" + GameManager.ScrollSpeed.ToString("F1");
        timeText.text = GameManager.CurrentTime.ToString("F3");
        beatText.text = GameManager.CurrentBeat.ToString("F0");
        bpmText.text = GameManager.CurrentBpm.ToString("F1") + " BPM";
    }

    public void VisualizeJudge(int line, JUDGES judge, float gap)
    {
        TriggerJudgeAnimation(judge);

        if (judge == JUDGES.BREAK)
        {
            EraseGap();
        }
        else if (judge == JUDGES.BAD)
        {
            ShowGap(gap);
            EraseGap();
        }
        else
        {
            ShowGap(gap);
            ShowScore(GameManager.Score);
            ShowCombo(GameManager.Combo);
            noteEffects[line].SetTrigger("Effect");
        }
    }

    public void VisualizeTickJudge(int line, JUDGES judge)
    {
        TriggerJudgeAnimation(judge);
        ShowCombo(GameManager.Combo);
        noteEffects[line].SetTrigger("Effect");
    }

    public void TriggerJudgeAnimation(JUDGES judge)
    {
        judgeAnimator.SetTrigger(judgeTriggers[(int)judge]);
    }

    public void ShowGap(float gap)
    {
        detailText.color = detailColor[(gap > 0) ? 1 : 0];
        detailText.text = ((gap > 0) ? "EARLY " : "LATE ") + (Math.Abs(gap) * 100).ToString("F0");
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
        ComboText.text = combo.ToString();
        comboAnimator.SetTrigger("Combo");
        comboHeadingAnimator.SetTrigger("ComboHeading");
    }
}
