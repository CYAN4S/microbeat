using System.Collections;
using System.Collections.Generic;
using System.IO;
using FileIO;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class Chart
    {
        public AudioClip audioClip;
        public SerializableDesc desc;
        public string directoryPath;
        public SerializablePattern pattern;

        public string musicName;
        public string artist;
        public string genre;

        public double BPMstandard;
        public BpmMeta BPMMeta;
        
        public Chart(SerializableDesc desc, string patternPath, string directoryPath)
        {
            this.desc = desc;
            this.directoryPath = directoryPath;
            pattern = FileExplorer.FromFile<SerializablePattern>(patternPath);
        }

        public IEnumerator SetAudioClip(UnityAction callback)
        {
            return FileExplorer.GetAudioClip(Path.Combine(directoryPath, desc.musicPath), value =>
            {
                audioClip = value;
                callback?.Invoke();
            });
        }
    }
}