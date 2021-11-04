using System.IO;
using Core.SO.NormalChannel;
using SO;
using UnityEngine;
using UnityEngine.Video;

namespace Gameplay.Visual
{
    public class MusicVideoPlayer : MonoBehaviour
    {
        [Header("Requirement")] [SerializeField]
        private PlayerSO player;

        [Header("Channel to get values from previous scene")] [SerializeField]
        private ChartEventChannelSO onChartSelect;

        private VideoPlayer videoPlayer;

        private void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            if (onChartSelect.value.desc.mvPath != null)
            {
                videoPlayer.url = Path.Combine(onChartSelect.value.directoryPath, onChartSelect.value.desc.mvPath);
            }
        }

        private void OnEnable()
        {
            player.ZeroEvent += PlayVideo;
            player.GamePauseEvent += PauseVideo;
            player.CountOverEvent += PlayVideo;
        }

        private void OnDisable()
        {
            player.ZeroEvent -= PlayVideo;
            player.GamePauseEvent -= PauseVideo;
            player.CountOverEvent -= PlayVideo;
        }

        private void PlayVideo()
        {
            videoPlayer.Play();
        }

        private void PauseVideo()
        {
            videoPlayer.Pause();
        }
    }
}