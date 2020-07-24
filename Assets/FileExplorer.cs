using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using UnityEngine;

public class FileExplorer : MonoBehaviour
{
    private string path;
    public List<(DirectoryInfo, SerializableDesc, FileInfo[])> musicData;

    private void Awake()
    {
        path = Application.persistentDataPath;
    }

    private void Start()
    {
        musicData = ExploreMusics();
        GetComponent<UIManager>().DisplayMusics();
    }

    public List<(DirectoryInfo, SerializableDesc, FileInfo[])> ExploreMusics()
    {
        var musicData = new List<(DirectoryInfo, SerializableDesc, FileInfo[])>();

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

            print(descFile.FullName);

            string text;
            using (StreamReader sr = descFile.OpenText())
            {
                text = sr.ReadToEnd();
            }

            musicData.Add((musicPack, DescFromJson(text), sheetFiles));
        }

        return musicData;
    }

    public SerializableDesc DescFromJson(string json)
    {
        return JsonUtility.FromJson<SerializableDesc>(json);
    }

}
