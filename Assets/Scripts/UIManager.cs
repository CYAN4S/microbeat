﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UIManager : MonoBehaviour
{
    public GameObject[] pressEffectObjects;
    public GameObject[] pressButtonObjects;
    public Text speedText, scoreText;
    public Text timeText;
    public Text detailText;
    public Animator judgeAnimator;
    public GameObject LIPrefab;
    public Transform UL;
    public Animator grooveLight;
    public Text comboText;

    public static readonly Color[] detailColor = { new Color(0, 222f / 256f, 1), new Color(1, 171f / 256f, 0) };
    public static readonly string[] judgeTriggers = { "Precise", "Great", "Nice", "Bad", "Break" };

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();

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

        GameManager.instance.OnMusicStart += () =>
        {
            StartGroove(gameManager.CurrentSheet.bpm);
        };

        GameManager.instance.OnJudge += (JUDGES judge, float gap) =>
        {
            TriggerJudgeAnimation(judge);

            if (judge == JUDGES.BREAK)
            {
                EraseGap();
                EraseCombo();
            }
            else if (judge == JUDGES.BAD)
            {
                ShowGap(gap);
                EraseGap();
                EraseCombo();
            }
            else
            {
                ShowGap(gap);
                ShowScore(GameManager.Score);
                ShowCombo(GameManager.Combo);
            }
        };

        GameManager.instance.OnGameEnd += () =>
        {
            StopGroove();
        };
    }

    private void LateUpdate()
    {
        if (!GameManager.IsWorking)
        {
            return;
        }

        speedText.text =
            gameManager.CurrentSheet.bpm.ToString("F0") + " X "
            + GameManager.ScrollSpeed.ToString("F2") + " = \n"
            + (gameManager.CurrentSheet.bpm * GameManager.ScrollSpeed).ToString("F2");
        timeText.text = GameManager.CurrentTime.ToString("F3");
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

    public void DisplayMusics()
    {
        var fe = GetComponent<FileExplorer>();
        int count = 0;
        foreach (var item in fe.musicData)
        {
            for (int i = 0; i < item.Item3.Count; i++)
            {
                SerializableSheet sheet = item.Item3[i];

                var target = Instantiate(LIPrefab, UL);
                target.transform.Translate(0, -250 * count++, 0);

                target.GetComponent<Button>().onClick.AddListener(() =>
                {
                    GameManager.instance.SheetSelect(item.Item1, item.Item2, sheet);
                    GetComponent<NongameUIManager>().selection.SetActive(false);
                });

                var lis = target.GetComponent<LISystem>();
                lis.title.text = item.Item2.name;
                lis.info.text = item.Item2.artist + " / " + item.Item2.genre;
                lis.level.text = CONST.PATTERN[sheet.pattern] + "\n" + sheet.level.ToString();
            }
        }
    }

    public void StartGroove(double bpm)
    {
        grooveLight.SetFloat("BPM", (float)bpm / 60f);
        grooveLight.SetTrigger("Begin");
    }

    public void StopGroove()
    {
        grooveLight.SetTrigger("End");
    }

    public void ShowCombo(int combo)
    {
        comboText.text = combo.ToString();
    }

    public void EraseCombo()
    {
        comboText.text = "";
    }
}
