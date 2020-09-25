using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class FileExplorer : MonoBehaviour
{
    public static FileExplorer Instance { get; private set; }
    public static string path;

    public List<Musicpack> musicpacks;
    public AudioClip streamAudio;

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
            FileInfo descFile = directory.GetFiles("info.mudesc")?[0];
            if (descFile == null)
            {
                continue;
            }

            FileInfo[] sheetFiles = directory.GetFiles("*.musheet");
            if (sheetFiles.Length == 0)
            {
                continue;
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

            musicpacks.Add(new Musicpack(directory, desc, sheets));
            yield return null;
        }

        callback?.Invoke();
    }

    public IEnumerator GetAudioClip(string path, Action callback = null)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            UnityWebRequestAsyncOperation x = www.SendWebRequest();
            x.completed += _ => callback?.Invoke();
            yield return x;

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //GetComponent<AudioSource>().clip = DownloadHandlerAudioClip.GetContent(www);
                streamAudio = DownloadHandlerAudioClip.GetContent(www);
            }
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