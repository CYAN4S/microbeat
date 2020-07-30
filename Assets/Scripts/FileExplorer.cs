using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

public class FileExplorer : MonoBehaviour
{
    private string path;
    public List<(DirectoryInfo, SerializableDesc, List<SerializableSheet>)> musicData;

    private void Awake()
    {
        path = Application.persistentDataPath;
    }

    private void Start()
    {
        StartCoroutine(Explore(GetComponent<UIManager>().DisplayMusics));
    }

    public IEnumerator Explore(Action callback = null)
    {
        musicData = new List<(DirectoryInfo, SerializableDesc, List<SerializableSheet>)>();

        DirectoryInfo musicPathInfo = new DirectoryInfo(Path.Combine(path, "Musics"));
        if (!musicPathInfo.Exists)
        {
            musicPathInfo.Create();
            yield break;
        }

        IEnumerable<DirectoryInfo> musicPacks = musicPathInfo.EnumerateDirectories();

        foreach (DirectoryInfo musicPack in musicPacks)
        {
            FileInfo descFile = musicPack.GetFiles("info.mudesc")?[0];
            if (descFile == null)
            {
                continue;
            }

            FileInfo[] sheetFiles = musicPack.GetFiles("*.musheet");
            if (sheetFiles.Length == 0)
            {
                continue;
            }

            string text;

            using (StreamReader sr = descFile.OpenText())
            {
                text = sr.ReadToEnd();
            }
            var desc = JsonUtility.FromJson<SerializableDesc>(text);

            List<SerializableSheet> sheets = new List<SerializableSheet>();
            foreach (var item in sheetFiles)
            {
                using (StreamReader sr = item.OpenText())
                {
                    text = sr.ReadToEnd();
                    sheets.Add(JsonUtility.FromJson<SerializableSheet>(text));
                }
            }

            musicData.Add((musicPack, desc, sheets));
            yield return null;
        }

        callback?.Invoke();
    }

    public IEnumerator GetAudioClip(string path, Action callback = null)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            var x = www.SendWebRequest();
            x.completed += _ => callback();
            yield return x;

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                GameManager.instance.AudioClip = DownloadHandlerAudioClip.GetContent(www);
            }
        }
    }
}
