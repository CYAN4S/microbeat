using System;
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

    public void ShowScore(double score)
    {
        scoreText.text = score.ToString("F0");
    }

    public void DisplayMusics()
    {
        print("RENDER BEGIN");
        var fe = GetComponent<FileExplorer>();
        foreach (var item in fe.musicData)
        {
            for (int i = 0; i < item.Item3.Count; i++)
            {
                SerializableSheet sheet = item.Item3[i];

                var target = Instantiate(LIPrefab, UL);
                target.transform.Translate(0, -250 * i, 0);

                print("RENDER: " + item.Item2.name);

                target.GetComponent<Button>().onClick.AddListener(() =>
                {
                    GetComponent<GameManager>().Launch(item.Item1, item.Item2, sheet);
                    GetComponent<NongameUIManager>().selection.SetActive(false);
                });

                var lis = target.GetComponent<LISystem>();
                lis.title.text = item.Item2.name;
                lis.info.text = item.Item2.artist + " / " + item.Item2.genre;
            }
        }
    }

}
