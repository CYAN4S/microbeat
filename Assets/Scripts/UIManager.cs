using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public RectTransform canvas;
    public GameObject selection, pause, result;

    public GameObject LIPrefab;
    public Transform UL;

    public GameObject[] pressEffectObjects;
    public GameObject[] pressButtonObjects;

    public Text speedText, scoreText;
    public Text timeText, beatText, bpmText;
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


    private void Start()
    {
        selection.SetActive(true);

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

        GameManager.Instance.OnSheetSelect += (_) =>
        {
            selection.SetActive(false);
        };

        GameManager.Instance.OnMusicStart += () =>
        {
            StartGroove(GameManager.Instance.Now.bpmMeta.std);
        };

        GameManager.Instance.OnJudge += (int line, JUDGES judge, float gap) =>
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

        GameManager.Instance.OnTickJudge += (int line, JUDGES judge) =>
        {
            TriggerJudgeAnimation(judge);
            ShowCombo(GameManager.Combo);
            noteEffects[line].SetTrigger("Effect");
        };

        GameManager.Instance.OnGameEnd += () =>
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
        beatText.text = GameManager.CurrentBeat.ToString("F0");
        bpmText.text = GameManager.CurrentBpm.ToString("F1") + " BPM";

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
        foreach (Musicpack music in FileExplorer.Instance.musicpacks)
        {
            for (int i = 0; i < music.sheets.Count; i++)
            {
                SerializableSheet sheet = music.sheets[i];

                GameObject gameObject = Instantiate(LIPrefab, UL);
                gameObject.transform.Translate(0, -250 * count++ * yMultiply, 0);

                gameObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    GameManager.Instance.OnSheetSelect(new SheetData
                    {
                        audioPath = Path.Combine(music.directory.FullName, music.desc.musicPath),
                        desc = music.desc,
                        sheet = sheet,
                    });
                    //GameManager.Instance.SheetSelect(music.desc, sheet, Path.Combine(music.directory.FullName, music.desc.musicPath));
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
        //comboTMPro.text = ComboToString(combo);
        comboTMPro.text = combo.ToString();
        comboAnimator.SetTrigger("Combo");
        comboHeadingAnimator.SetTrigger("ComboHeading");
    }

    public string ComboToString(int combo)
    {
        const string prefix = "<style=\"CB\">", suffix = ">";
        string result = "";

        foreach (char item in combo.ToString())
        {
            result = result + prefix + item + suffix;
        }

        return result;
    }

    public void DisplayResult()
    {
        for (int i = 0; i < GameManager.Instance.JudgeCounts.Length; i++)
        {
            judgeCountTexts[i].text = GameManager.Instance.JudgeCounts[i].ToString();
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
