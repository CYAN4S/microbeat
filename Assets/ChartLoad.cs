using System;
using System.Collections;
using System.IO;
using Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ChartLoad : MonoBehaviour
{
    [SerializeField] private ChartPathEventChannelSO chartSelectF;
    
    [SerializeField] private ChartEventChannelSO loadFinishedI;
    
    private ChartPath chartPath;
    private Chart chart;
    private AudioClip audioClip;

    // private void OnEnable()
    // {
    //     audioLoadI.onEventRaised += LoadFinished;
    // }
    //
    // private void OnDisable()
    // {
    //     audioLoadI.onEventRaised -= LoadFinished;
    // }

    private void Start()
    {
        chartPath = chartSelectF.value;
        chart = new Chart();

        if (chartPath == null)
        {
            Debug.LogError("No ChartPath in channel.");
            return;
        }

        chart.desc = FileExplorer.FromFile<SerializableDesc>(chartPath.descPath);
        chart.pattern = FileExplorer.FromFile<SerializablePattern>(chartPath.patternPath);
        StartCoroutine(GetAudioClip(chartPath.audioPath, LoadFinished));
        Debug.Log(chart.desc.artist + " / " + chartPath.audioPath);
    }

    private void LoadFinished()
    {
        Debug.Log(audioClip.length);
        chart.audioClip = audioClip;
        loadFinishedI.RaiseEvent(chart);
        SceneManager.LoadScene(3);
    }
    
    public IEnumerator GetAudioClip(string audioPath, UnityAction callback = null)
    {
        using var www = UnityWebRequestMultimedia.GetAudioClip(audioPath, AudioType.WAV);
        var x = www.SendWebRequest();
        x.completed += _ => callback?.Invoke();
        yield return x;
    
        if (www.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(www.error);
        else
            audioClip = DownloadHandlerAudioClip.GetContent(www);
    }
}