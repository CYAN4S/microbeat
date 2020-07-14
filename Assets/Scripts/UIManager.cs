using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject[] pressEffectObjects;
    public GameObject[] pressButtonObjects;
    public Text speedText, scoreText;
    public Text timeText;
    public Text detailText;
    public Animator judgeAnimator;

    public static readonly Color[] detailColor = { new Color(0, 222f / 256f, 1), new Color(1, 171f / 256f, 0) };
    public static readonly string[] judgeTriggers = { "Precise", "Great", "Nice", "Bad", "Break" };

    private void Awake()
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

        speedText.text = GameManager.ScrollSpeed.ToString("F2");
        timeText.text = GameManager.CurrentTime.ToString("F3");
    }

    public void LaunchJudge(JUDGES judge)
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

}
