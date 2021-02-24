using FileIO;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MusicListContent : MonoBehaviour
    {
        public MusicData musicData;
        public Text title;
        public Text info;

        public void SetValue(MusicData target)
        {
            musicData = target;
            title.text = musicData.desc.name;
            info.text = $"{musicData.desc.artist} / {musicData.desc.genre}";
        }
    }
}