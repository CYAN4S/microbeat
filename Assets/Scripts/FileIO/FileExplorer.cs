using System;
using System.Collections;
using System.IO;
using Events;
using UnityEngine;
using UnityEngine.Networking;

public class FileExplorer : MonoBehaviour
{
    public static string path;

    public AudioClip streamAudio;

    [SerializeField] private ChartPathEventChannelSO chartPathLoadedI;
    // [SerializeField] private VoidEventChannelSO startExploreF;

    private void Awake()
    {
        path = Application.persistentDataPath;
    }

    private void Start()
    {
        StartExplore();
    }

    private void OnEnable()
    {
        //startExploreF.onEventRaised += StartExplore;
    }

    private void OnDisable()
    {
        //startExploreF.onEventRaised -= StartExplore;
    }

    private void StartExplore()
    {
        StartCoroutine(ExploreAsync());
    }

    public IEnumerator ExploreAsync(Action callback = null)
    {
        // musicpacks = new List<Musicpack>();
        var musicDirectory = new DirectoryInfo(Path.Combine(path, "Musics"));

        if (!musicDirectory.Exists)
        {
            musicDirectory.Create();
            yield break;
        }

        var directories = musicDirectory.EnumerateDirectories();
        foreach (var directory in directories)
        {
            SeekDirectory(directory);
            yield return null;
        }

        callback?.Invoke();
    }

    private void SeekDirectory(DirectoryInfo directory)
    {
        var descFile = directory.GetFiles("info.mudesc")?[0];
        if (descFile == null) return;

        var patternFiles = directory.GetFiles("*.musheet");
        if (patternFiles.Length == 0) return;

        var desc = FromFile<SerializableDesc>(descFile);

        foreach (var patternFile in patternFiles)
        {
            var pattern = FromFile<SerializablePattern>(patternFile);
            var chartPath = new ChartPath(
                Path.Combine(directory.FullName, desc.musicPath),
                descFile.FullName,
                patternFile.FullName,
                desc.name,
                desc.artist,
                desc.genre,
                pattern.line,
                pattern.level,
                pattern.diff
            );

            chartPathLoadedI.RaiseEvent(chartPath);
        }
    }

    public static T FromFile<T>(FileInfo file)
    {
        using var sr = file.OpenText();
        return JsonUtility.FromJson<T>(sr.ReadToEnd());
    }

    public static T FromFile<T>(string filePath)
    {
        using var sr = File.OpenText(filePath);
        return JsonUtility.FromJson<T>(sr.ReadToEnd());
    }

    public IEnumerator GetAudioClip(string audioPath, Action callback = null)
    {
        using var www = UnityWebRequestMultimedia.GetAudioClip(audioPath, AudioType.WAV);
        var x = www.SendWebRequest();
        x.completed += _ => callback?.Invoke();
        yield return x;
    
        if (www.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(www.error);
        else
            streamAudio = DownloadHandlerAudioClip.GetContent(www);
    }

    public static IEnumerator GetAudioClip(string audioPath, AudioClipEventChannelSO channel)
    {
        using var www = UnityWebRequestMultimedia.GetAudioClip(audioPath, AudioType.WAV);
        yield return www.SendWebRequest();
        Debug.Log(audioPath);

        if (www.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(www.error);
        else
            channel.RaiseEvent(DownloadHandlerAudioClip.GetContent(www));
    }
}

public class ChartPath
{
    public string artist;
    public string audioPath;
    public string descPath;
    public int diff;
    public string genre;
    public int level;
    public int line;

    public string name;
    public string patternPath;

    public ChartPath(string audioPath, string descPath, string patternPath, string name, string artist, string genre,
        int line, int level, int diff)
    {
        this.audioPath = audioPath;
        this.descPath = descPath;
        this.patternPath = patternPath;
        this.name = name;
        this.artist = artist;
        this.genre = genre;
        this.line = line;
        this.level = level;
        this.diff = diff;
    }
}

public struct ChartData
{
    public string audioPath;
    public SerializableDesc desc;
    public SerializablePattern pattern;
}