using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MusicListController : MonoBehaviour
{
    private int count = 0;
    private float yMultiply;
    [SerializeField] private FileIOChannelSO channel;
    public RectTransform canvas;
    public GameObject LIPrefab;
    public RectTransform ScrollViewportContent;

    private void Start()
    { 
        yMultiply = canvas.localScale.y;
    }

    private void OnEnable()
    {
        channel.chartLoadedEvent += AddChart;
    }

    private void OnDisable()
    {
        channel.chartLoadedEvent -= AddChart;
    }

    private void AddChart(Musicpack music, SerializableSheet sheet)
    {
        var obj = Instantiate(LIPrefab, ScrollViewportContent);
        obj.transform.Translate(0, -250 * count++ * yMultiply, 0);
        ScrollViewportContent.sizeDelta = new Vector2(0, 250 * count);
        
        obj.GetComponent<Button>().onClick.AddListener(() =>
        {
            GameManager.Instance.OnSheetSelect(new ChartData
            {
                audioPath = Path.Combine(music.directory.FullName, music.desc.musicPath),
                desc = music.desc,
                sheet = sheet,
            });
        });
        
        LISystem liSystem = obj.GetComponent<LISystem>();
        liSystem.title.text = music.desc.name;
        liSystem.info.text = music.desc.artist + " / " + music.desc.genre;
        liSystem.level.text = CONST.PATTERN[sheet.pattern] + "\n" + sheet.level.ToString();
    }

    private void OnChartSelect()
    {
        
    }
}
