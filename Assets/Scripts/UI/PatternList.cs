using System.Collections.Generic;
using Core;
using FileIO;
using SO.NormalChannel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class PatternList : MonoBehaviour
    {
        [Header("Channel to follow")] [SerializeField]
        private MusicDataEventChannelSO onMusicDataSelect;

        [Header("Channel to invoke")] [SerializeField]
        private ChartEventChannelSO onChartSelect;

        [Header("Requirement")] [SerializeField]
        private RectTransform canvas;

        [SerializeField] private GameObject listContentPrefab;
        [SerializeField] private RectTransform listContainer;

        private int count;
        private float yMultiply;

        private List<GameObject> objects = new List<GameObject>();

        private void Start()
        {
            yMultiply = canvas.localScale.y;
        }

        private void OnEnable()
        {
            onMusicDataSelect.OnEventRaised += RefreshPatternList;
        }

        private void OnDisable()
        {
            onMusicDataSelect.OnEventRaised -= RefreshPatternList;
        }

        private void RefreshPatternList(MusicData musicData)
        {
            foreach (var obj in objects)
            {
                GameObject.Destroy(obj);
                count = 0;
            }

            foreach (var tuple in musicData.patternData)
            {
                var obj = Instantiate(listContentPrefab, listContainer);
                obj.transform.Translate(190 * count++ * yMultiply, 0, 0);

                obj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var chart = new Chart(musicData.desc, tuple.Item1, musicData.path);
                    StartCoroutine(chart.SetAudioClip(() =>
                    {
                        onChartSelect.RaiseEvent(chart);
                        SceneManager.LoadScene(2);
                    }));
                });
                obj.GetComponent<PatternListContent>().SetValue(tuple.Item2, tuple.Item3, tuple.Item4);
                objects.Add(obj);
            }
        }
    }
}