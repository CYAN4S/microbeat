using Core;
using SO;
using SO.NormalChannel;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Result : MonoBehaviour
    {
        [SerializeField] private PlayerSO player;
        [SerializeField] private ChartEventChannelSO onChartSelect;

        [SerializeField] private Text[] judgeCountTexts;
        [SerializeField] private Text resultText;
        [SerializeField] private Text musicText;

        private void Start()
        {
            DisplayResult();
        }

        public void DisplayResult()
        {
            for (var i = 0; i < player.JudgeCounts.Length; i++)
                judgeCountTexts[i].text = player.JudgeCounts[i].ToString();

            var rank = Const.RANK_NAME[Const.RANK_NAME.Length - 1];
            for (var i = 0; i < Const.RANK.Length; i++)
                if (player.Score >= Const.RANK[i])
                {
                    rank = Const.RANK_NAME[i];
                    break;
                }

            resultText.text = player.Score.ToString("F0") + " / RANK " + rank;
            musicText.text = onChartSelect.value.desc.name + " / " + onChartSelect.value.desc.artist;
        }
    }
}