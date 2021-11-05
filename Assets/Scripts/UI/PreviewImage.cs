using System.IO;
using FileIO;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PreviewImage : MonoBehaviour
    {
        [Header("Requirement")] 
        [SerializeField] private RawImage rawImage;
        [SerializeField] private Texture2D defaultImage;

        [Header("Channel to follow")] 
        [SerializeField] private MusicDataEventChannelSO onMusicDataSelect;

        [Header("Extra")] 
        [SerializeField] private Text title;

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
            //TEMP//
            if (title)
            {
                title.text = musicData.desc.name;
            }
            
            if (musicData.desc.previewImgPath == null)
            {
                rawImage.texture = defaultImage;
                return;
            }

            var path = Path.Combine(musicData.path, musicData.desc.previewImgPath);
            StartCoroutine(Serialize.GetTexture(path,
                (value) => { rawImage.texture = value; }));
        }
    }
}