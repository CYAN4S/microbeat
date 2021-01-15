using Events;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static readonly Color[] DETAIL_COLOR = {new Color(0, 222f / 256f, 1), new Color(1, 171f / 256f, 0)};
    public static readonly string[] JUDGE_TRIGGERS = {"Precise", "Great", "Nice", "Bad", "Break"};
    public RectTransform canvas;
    public GameObject pause, result;

    public Text[] judgeCountTexts;
    public Text resultText;

    [SerializeField] private PlayerSO player;


    public void DisplayResult()
    {
        for (var i = 0; i < player.JudgeCounts.Length; i++)
            judgeCountTexts[i].text = player.JudgeCounts[i].ToString();

        var rank = CONST.RANK_NAME[CONST.RANK_NAME.Length - 1];
        for (var i = 0; i < CONST.RANK.Length; i++)
            if (player.Score >= CONST.RANK[i])
            {
                rank = CONST.RANK_NAME[i];
                break;
            }

        resultText.text = player.Score.ToString("F0") + " / RANK " + rank;

        result.SetActive(true);
    }
}