using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class FileExplorer : MonoBehaviour
{
    public static FileExplorer Instance { get; private set; }
    public static string path;

    public List<Musicpack> musicpacks;
    public AudioClip streamAudio;

    [SerializeField] private FileIOChannelSO channel;
    [SerializeField] private VoidEventChannelSO startExploreEventChannel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        path = Application.persistentDataPath;
    }

    private void OnEnable()
    {
        startExploreEventChannel.onEventRaised += StartExplore;
    }

    private void OnDisable()
    {
        startExploreEventChannel.onEventRaised -= StartExplore;
    }

    private void StartExplore()
    {
        StartCoroutine(ExploreAsync());
    }

    public IEnumerator ExploreAsync(Action callback = null)
    {
        musicpacks = new List<Musicpack>();
        DirectoryInfo musicDirectory = new DirectoryInfo(Path.Combine(path, "Musics"));

        if (!musicDirectory.Exists)
        {
            musicDirectory.Create();
            yield break;
        }

        IEnumerable<DirectoryInfo> directories = musicDirectory.EnumerateDirectories();
        foreach (DirectoryInfo directory in directories)
        {
            SeekDirectory(directory);
            yield return null;
        }

        callback?.Invoke();
    }

    private void SeekDirectory(DirectoryInfo directory)
    {
        FileInfo descFile = directory.GetFiles("info.mudesc")?[0];
        if (descFile == null)
        {
            return;
        }

        FileInfo[] sheetFiles = directory.GetFiles("*.musheet");
        if (sheetFiles.Length == 0)
        {
            return;
        }

        SerializableDesc desc;
        using (StreamReader sr = descFile.OpenText())
        {
            desc = JsonUtility.FromJson<SerializableDesc>(sr.ReadToEnd());
        }

        List<SerializableSheet> sheets = new List<SerializableSheet>();
        foreach (FileInfo item in sheetFiles)
        {
            using (StreamReader sr = item.OpenText())
            {
                sheets.Add(JsonUtility.FromJson<SerializableSheet>(sr.ReadToEnd()));
            }
        }

        var newMusic = new Musicpack(directory, desc, sheets);
        foreach (var sheet in newMusic.sheets)
        {
            channel.OnChartLoaded(newMusic, sheet);
        }
        musicpacks.Add(newMusic);
    }

    public IEnumerator GetAudioClip(string audioPath, Action callback = null)
    {
        using var www = UnityWebRequestMultimedia.GetAudioClip(audioPath, AudioType.WAV);
        var x = www.SendWebRequest();
        x.completed += _ => callback?.Invoke();
        yield return x;

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            streamAudio = DownloadHandlerAudioClip.GetContent(www);
        }
    }
}

public struct Musicpack
{
    public DirectoryInfo directory;
    public SerializableDesc desc;
    public List<SerializableSheet> sheets;

    public Musicpack(DirectoryInfo item1, SerializableDesc item2, List<SerializableSheet> item3)
    {
        directory = item1;
        desc = item2;
        sheets = item3;
    }
}

public struct ChartData
{
    public string audioPath;
    public SerializableDesc desc;
    public SerializableSheet sheet;
}