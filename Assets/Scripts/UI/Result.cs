using Events;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [SerializeField] private PlayerSO player;

    [SerializeField] private Text[] judgeCountTexts;
    [SerializeField] private Text resultText;

    private void Start()
    {
        DisplayResult();
    }

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
    }
}