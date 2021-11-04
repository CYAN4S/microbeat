using System;
using System.Collections;
using System.IO;
using FileIO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Gameplay
{
    public class FileExplorer : MonoBehaviour
    {
        public static string path;

        [Header("Channel to invoke")] [SerializeField]
        private MusicDataEventChannelSO onMusicDataLoad;

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

            var desc = Serialize.FromFile<SerializableDesc>(descFile);
            var md = new MusicData(desc, directory.FullName);

            foreach (var patternFile in patternFiles)
            {
                var pattern = Serialize.FromFile<SerializablePattern>(patternFile);
                md.AddTuple(patternFile.FullName, pattern.line, pattern.level, pattern.diff);
            }

            onMusicDataLoad.RaiseEvent(md);
        }
    }
}