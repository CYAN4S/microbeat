using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MusicListController : MonoBehaviour
    {
        [Header("Channel to follow")] [SerializeField]
        private MusicDataEventChannelSO onMusicDataLoad;

        [Header("Channel to invoke")] [SerializeField]
        private MusicDataEventChannelSO onMusicDataSelect;

        [Header("Requirement")] [SerializeField]
        private RectTransform canvas;

        [SerializeField] private GameObject listContentPrefab;
        [SerializeField] private RectTransform listContainer;

        private int count;
        private float yMultiply;

        private void Start()
        {
            yMultiply = canvas.localScale.y;
        }

        private void OnEnable()
        {
            onMusicDataLoad.OnEventRaised += AddMusicData;
        }

        private void OnDisable()
        {
            onMusicDataLoad.OnEventRaised -= AddMusicData;
        }

        private void AddMusicData(MusicData musicData)
        {
            var obj = Instantiate(listContentPrefab, listContainer);
            obj.transform.Translate(0, -120 * count++ * yMultiply, 0);
            listContainer.sizeDelta = new Vector2(0, 120 * count);

            obj.GetComponent<Button>().onClick.AddListener(() => { onMusicDataSelect.RaiseEvent(musicData); });
            obj.GetComponent<MusicListContent>().SetValue(musicData);
        }
    }
}