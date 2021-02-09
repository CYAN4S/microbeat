using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SO;
using SO.NormalChannel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace FileIO
{
    public class FileExplorer : MonoBehaviour
    {
        public static string path;
        
        [Header("Channel to invoke")]
        [SerializeField] private MusicDataEventChannelSO onMusicDataLoad;

        private void Awake()
        {
            path = Application.persistentDataPath;
        }

        private void Start()
        {
            StartExplore();
        }

        private void StartExplore()
        {
            StartCoroutine(ExploreAsync());
        }

        public IEnumerator ExploreAsync(Action callback = null)
        {
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
            var descs = directory.GetFiles(".mcbdesc");
            if (descs.Length == 0) return;
            var descFile = descs[0];

            var patternFiles = directory.GetFiles("*.mcbchart");
            if (patternFiles.Length == 0) return;

            var desc = FromFile<SerializableDesc>(descFile);
            var md = new MusicData(desc, directory.FullName);

            foreach (var patternFile in patternFiles)
            {
                var pattern = FromFile<SerializablePattern>(patternFile);
                md.AddTuple(patternFile.FullName, pattern.line, pattern.level, pattern.diff);
            }

            onMusicDataLoad.RaiseEvent(md);
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

        public static IEnumerator GetAudioClip(string audioPath, UnityAction<AudioClip> callback)
        {
            using var www = UnityWebRequestMultimedia.GetAudioClip(audioPath, AudioType.WAV);
            var x = www.SendWebRequest();
            yield return x;

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error + " / " + audioPath);
            }
            else
            {
                callback.Invoke(DownloadHandlerAudioClip.GetContent(www));
            }
        }

        public static IEnumerator GetTexture(string imgPath, UnityAction<Texture2D> callback)
        {
            using var uwr = UnityWebRequestTexture.GetTexture(imgPath);
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error + " / " + imgPath);
            }
            else
            {
                callback.Invoke(DownloadHandlerTexture.GetContent(uwr));
            }
        }
    }

    public class MusicData
    {
        public string path;
        public SerializableDesc desc;
        // audio path, line, level, diff
        public List<Tuple<string, int, int, int>> patternData;

        public MusicData(SerializableDesc desc, string path)
        {
            this.desc = desc;
            this.path = path;
            this.patternData = new List<Tuple<string, int, int, int>>();
        }

        public void AddTuple(string path, int line, int level, int diff)
        {
            patternData.Add(new Tuple<string, int, int, int>(path, line, level, diff));
        }
    }

}