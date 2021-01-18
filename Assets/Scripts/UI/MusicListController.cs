using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicListController : MonoBehaviour
{
    [SerializeField] private ChartPathEventChannelSO chartPathLoadedF;
    [SerializeField] private ChartPathEventChannelSO chartSelectI;
    public RectTransform canvas;
    public GameObject LIPrefab;
    public RectTransform ScrollViewportContent;
    private int count;
    private float yMultiply;

    private void Start()
    {
        yMultiply = canvas.localScale.y;
    }

    private void OnEnable()
    {
        chartPathLoadedF.onEventRaised += AddChartPath;
    }

    private void OnDisable()
    {
        chartPathLoadedF.onEventRaised -= AddChartPath;
    }

    private void AddChartPath(ChartPath chartPath)
    {
        var obj = Instantiate(LIPrefab, ScrollViewportContent);
        obj.transform.Translate(0, -250 * count++ * yMultiply, 0);
        ScrollViewportContent.sizeDelta = new Vector2(0, 250 * count);

        obj.GetComponent<Button>().onClick.AddListener(() =>
        {
            chartSelectI.RaiseEvent(chartPath);
            SceneManager.LoadScene(2);
        });

        var liSystem = obj.GetComponent<LISystem>();
        liSystem.title.text = chartPath.name;
        liSystem.info.text = chartPath.artist + " / " + chartPath.genre;
        liSystem.level.text = CONST.PATTERN[chartPath.diff] + "\n" + chartPath.level;
    }

    private void OnChartSelect()
    {
    }
}