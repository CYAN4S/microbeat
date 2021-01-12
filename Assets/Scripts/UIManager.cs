using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static readonly Color[] DETAIL_COLOR = {new Color(0, 222f / 256f, 1), new Color(1, 171f / 256f, 0)};
    public static readonly string[] JUDGE_TRIGGERS = {"Precise", "Great", "Nice", "Bad", "Break"};
    public RectTransform canvas;
    public GameObject selection, pause, result;

    public Text[] judgeCountTexts;
    public Text resultText;


    private void Start()
    {
        selection.SetActive(true);
    }

    public void DisplayResult()
    {
        for (var i = 0; i < GameManager.Instance.JudgeCounts.Length; i++)
            judgeCountTexts[i].text = GameManager.Instance.JudgeCounts[i].ToString();

        var rank = CONST.RANKNAME[CONST.RANKNAME.Length - 1];
        for (var i = 0; i < CONST.RANK.Length; i++)
            if (GameManager.Score >= CONST.RANK[i])
            {
                rank = CONST.RANKNAME[i];
                break;
            }

        resultText.text = GameManager.Score.ToString("F0") + " / RANK " + rank;

        result.SetActive(true);
    }
}