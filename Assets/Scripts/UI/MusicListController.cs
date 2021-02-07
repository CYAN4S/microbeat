using Core;
using FileIO;
using SO;
using SO.NormalChannel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MusicListController : MonoBehaviour
    {
        [Header("Channel to follow")]
        [SerializeField] private ChartPathEventChannelSO onLoadFinish;
        [SerializeField] private MusicDataEventChannelSO onMusicDataLoad;
        
        [Header("Channel to invoke")]
        [SerializeField] private ChartPathEventChannelSO onMusicSelect;
        
        [Header("Requirement")]
        [SerializeField] private RectTransform canvas;
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
            onLoadFinish.OnEventRaised += AddChartPath;
        }

        private void OnDisable()
        {
            onLoadFinish.OnEventRaised -= AddChartPath;
        }

        private void AddChartPath(ChartPath chartPath)
        {
            var obj = Instantiate(listContentPrefab, listContainer);
            obj.transform.Translate(0, -120 * count++ * yMultiply, 0);
            listContainer.sizeDelta = new Vector2(0, 120 * count);

            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                onMusicSelect.RaiseEvent(chartPath);
                SceneManager.LoadScene(2);
            });

            var liSystem = obj.GetComponent<LISystem>();
            liSystem.title.text = chartPath.name;
            liSystem.info.text = chartPath.artist + " / " + chartPath.genre;
            liSystem.level.text = Const.PATTERN[chartPath.diff] + " " + chartPath.level;
        }

        private void AddMusicData(MusicData musicData)
        {
            
        }

        private void OnChartSelect()
        {
        }
    }
}