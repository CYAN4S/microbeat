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
        musicData = ExploreMusics();
        GetComponent<UIManager>().DisplayMusics();
    }

    public List<(DirectoryInfo, SerializableDesc, List<SerializableSheet>)> ExploreMusics()
    {
        var musicData = new List<(DirectoryInfo, SerializableDesc, List<SerializableSheet>)>();

        string musicPath = Path.Combine(path, "Musics");

        DirectoryInfo musicPathInfo = new DirectoryInfo(musicPath);
        if (!musicPathInfo.Exists)
        {
            musicPathInfo.Create();
            return musicData;
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
            var desc = FromJson<SerializableDesc>(text);

            List<SerializableSheet> sheets = new List<SerializableSheet>();
            foreach (var item in sheetFiles)
            {
                using (StreamReader sr = item.OpenText())
                {
                    text = sr.ReadToEnd();
                    sheets.Add(FromJson<SerializableSheet>(text));
                }
            }

            print(musicPack.FullName + " / " + desc.name + " / " + sheets.Count.ToString() + " file(s).");
            musicData.Add((musicPack, desc, sheets));
        }

        return musicData;
    }

    public T FromJson<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }

    public static AudioClip GetAudioClip(string path)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
                return null;
            }
            else
            {
                return DownloadHandlerAudioClip.GetContent(www);
            }
        }
    }
}
