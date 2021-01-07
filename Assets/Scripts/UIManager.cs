using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public RectTransform canvas;
    public GameObject selection, pause, result;

    // public GameObject LIPrefab;
    // public Transform UL;
    // public RectTransform ScrollViewportContent;

    public Text[] judgeCountTexts;
    public Text resultText;

    public static readonly Color[] detailColor = { new Color(0, 222f / 256f, 1), new Color(1, 171f / 256f, 0) };
    public static readonly string[] judgeTriggers = { "Precise", "Great", "Nice", "Bad", "Break" };


    private void Start()
    {
        selection.SetActive(true);
    }

    // public void DisplayMusics()
    // {
    //     int count = 0;
    //     float yMultiply = canvas.localScale.y;
    //     foreach (Musicpack music in FileExplorer.Instance.musicpacks)
    //     {
    //         for (int i = 0; i < music.sheets.Count; i++)
    //         {
    //             SerializableSheet sheet = music.sheets[i];
    //             GameObject gameObject = Instantiate(LIPrefab, ScrollViewportContent);
    //             gameObject.transform.Translate(0, -250 * count++ * yMultiply, 0);
    //             ScrollViewportContent.sizeDelta = new Vector2(0, 250 * count);
    //
    //             gameObject.GetComponent<Button>().onClick.AddListener(() =>
    //             {
    //                 GameManager.Instance.OnSheetSelect(new ChartData
    //                 {
    //                     audioPath = Path.Combine(music.directory.FullName, music.desc.musicPath),
    //                     desc = music.desc,
    //                     sheet = sheet,
    //                 });
    //             });
    //
    //             LISystem LiSystem = gameObject.GetComponent<LISystem>();
    //             LiSystem.title.text = music.desc.name;
    //             LiSystem.info.text = music.desc.artist + " / " + music.desc.genre;
    //             LiSystem.level.text = CONST.PATTERN[sheet.pattern] + "\n" + sheet.level.ToString();
    //         }
    //     }
    // }

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
