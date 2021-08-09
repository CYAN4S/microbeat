using System.Collections;
using System.IO;
using FileIO;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class Chart
    {
        public AudioClip audioClip;
        // public SerializableDesc desc;
        public string path;
        // public SerializablePattern pattern;

        public SerializableDescriptionData descriptionData;
        public SerializablePatternData patternData;

        public Chart(SerializableDesc desc, string patternPath, string path)
        {
            // this.desc = desc;
            this.path = path;
            // pattern = FileExplorer.FromFile<SerializablePattern>(patternPath);

            this.descriptionData = SerializableDescriptionData.UpgradeFrom(desc);
            this.patternData = SerializablePatternData.UpgradeFrom(FileExplorer.FromFile<SerializablePattern>(patternPath));
        }

        public IEnumerator SetAudioClip(UnityAction callback)
        {
            return FileExplorer.GetAudioClip(Path.Combine(path, descriptionData.musicPath), value =>
            {
                audioClip = value;
                callback?.Invoke();
            });
        }
    }
}