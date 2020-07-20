using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileExplorer : MonoBehaviour
{
    private string path;

    private void Awake()
    {
        path = Application.persistentDataPath;
        print(path);
    }

    public void ExploreMusics()
    {
        string musicPath = Path.Combine(path, "Musics");
        if (!Directory.Exists(musicPath))
        {
            Directory.CreateDirectory(musicPath);
            return;
        }
    }
}
