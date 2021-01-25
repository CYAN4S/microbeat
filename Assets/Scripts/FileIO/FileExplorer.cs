using System;
using System.Collections;
using System.IO;
using SO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace FileIO
{
    public class FileExplorer : MonoBehaviour
    {
        public static string path;
        [SerializeField] private ChartPathEventChannelSO chartPathLoadedI;

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

        public static IEnumerator GetAudioClip(string audioPath, UnityAction<AudioClip> callback)
        {
            using var www = UnityWebRequestMultimedia.GetAudioClip(audioPath, AudioType.WAV);
            var x = www.SendWebRequest();
            yield return x;

            if (www.result == UnityWebRequest.Result.ConnectionError)
                Debug.Log(www.error);
            else
                //streamAudio = DownloadHandlerAudioClip.GetContent(www);
                callback.Invoke(DownloadHandlerAudioClip.GetContent(www));
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
}