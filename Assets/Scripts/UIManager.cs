using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class UIManager : MonoBehaviour
{
    public RectTransform canvas;
    public GameObject selection, pause, result;

    public GameObject LIPrefab;
    public Transform UL;

    public GameObject[] pressEffectObjects;
    public GameObject[] pressButtonObjects;

    public Text speedText, scoreText;
    public Text timeText;
    public Text detailText;

    public Animator judgeAnimator;
    public Animator grooveLight;
    public Animator comboAnimator;
    public Animator comboHeadingAnimator;
    public Animator[] noteEffects;

    public TextMeshProUGUI comboTMPro;

    public Text[] judgeCountTexts;
    public Text resultText;

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

    private void Start()
    {
        selection.SetActive(true);

        GameManager.instance.OnSheetSelect += () =>
        {
            selection.SetActive(false);
        };

        GameManager.instance.OnMusicStart += () =>
        {
            StartGroove(GameManager.instance.CurrentSheet.bpm);
        };

        GameManager.instance.OnJudge += (int line, JUDGES judge, float gap) =>
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
        };

        GameManager.instance.OnGameEnd += () =>
        {
            StopGroove();
            DisplayResult();
        };
    }

    private void LateUpdate()
    {
        if (!GameManager.IsWorking)
        {
            return;
        }

        speedText.text = "X" + GameManager.ScrollSpeed.ToString("F1");
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
        int count = 0;
        float yMultiply = canvas.localScale.y;
        foreach (Music music in FileExplorer.Instance.musicData)
        {
            for (int i = 0; i < music.sheets.Count; i++)
            {
                SerializableSheet sheet = music.sheets[i];

                GameObject gameObject = Instantiate(LIPrefab, UL);
                gameObject.transform.Translate(0, -250 * count++ * yMultiply, 0);

                gameObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    GameManager.instance.SheetSelect(music.desc, sheet, Path.Combine(music.directory.FullName, music.desc.musicPath));
                });

                LISystem LiSystem = gameObject.GetComponent<LISystem>();
                LiSystem.title.text = music.desc.name;
                LiSystem.info.text = music.desc.artist + " / " + music.desc.genre;
                LiSystem.level.text = CONST.PATTERN[sheet.pattern] + "\n" + sheet.level.ToString();
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
        comboTMPro.text = ComboToString(combo);
        comboAnimator.SetTrigger("Combo");
        comboHeadingAnimator.SetTrigger("ComboHeading");
    }

    public string ComboToString(int combo)
    {
        const string prefix = "<style=\"CB\">", suffix = ">";
        string result = "";

        foreach (var item in combo.ToString())
        {
            result = result + prefix + item + suffix;
        }

        return result;
    }

    public void DisplayResult()
    {
        for (int i = 0; i < GameManager.instance.JudgeCounts.Length; i++)
        {
            judgeCountTexts[i].text = GameManager.instance.JudgeCounts[i].ToString();
        }

        string rank = CONST.RANKNAME[CONST.RANKNAME.Length - 1];
        for (int i = 0; i < CONST.RANK.Length; i++)
        {
            if (GameManager.Score >= CONST.RANK[i])
            {
                rank = CONST.RANKNAME[i];
                break;
            }
        }

        resultText.text = GameManager.Score.ToString("F0") + " / RANK " + rank;

        result.SetActive(true);
    }
}
