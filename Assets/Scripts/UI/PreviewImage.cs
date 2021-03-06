using System.IO;
using FileIO;
using SO.NormalChannel;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PreviewImage : MonoBehaviour
    {
        [Header("Requirement")] [SerializeField]
        private RawImage rawImage;

        [SerializeField] private Texture2D defaultImage;

        [Header("Channel to follow")] [SerializeField]
        private MusicDataEventChannelSO onMusicDataSelect;

        private void OnEnable()
        {
            onMusicDataSelect.OnEventRaised += ChangeImage;
        }

        private void OnDisable()
        {
            onMusicDataSelect.OnEventRaised -= ChangeImage;
        }

        private void ChangeImage(MusicData musicData)
        {
            if (musicData.desc.previewImgPath == null)
            {
                rawImage.texture = defaultImage;
                return;
            }

            var path = Path.Combine(musicData.path, musicData.desc.previewImgPath);
            StartCoroutine(FileExplorer.GetTexture(path,
                (value) => { rawImage.texture = value; }));
        }
    }
}