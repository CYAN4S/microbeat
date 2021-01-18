using System.Collections;
using Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ChartLoad : MonoBehaviour
{
    [SerializeField] private ChartPathEventChannelSO chartSelectF;

    [SerializeField] private ChartEventChannelSO loadFinishedI;
    private AudioClip audioClip;
    private Chart chart;

    private ChartPath chartPath;

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
    }

    private void LoadFinished()
    {
        chart.audioClip = audioClip;
        loadFinishedI.RaiseEvent(chart);
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