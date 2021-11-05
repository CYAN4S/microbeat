using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace FileIO
{
    public class Serialize
    {
        public static T FromFile<T>(FileInfo file) where T : class
        {
            try
            {
                using var sr = file.OpenText();
                return JsonUtility.FromJson<T>(sr.ReadToEnd());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static T FromFile<T>(string filePath) where T : class
        {
            try
            {
                using var sr = File.OpenText(filePath);
                return JsonUtility.FromJson<T>(sr.ReadToEnd());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static void ToFile<T>(T target, string filePath)
        {
            using var stream = new StreamWriter(filePath);
            stream.Write(JsonUtility.ToJson(target));
        }

        public static IEnumerator GetAudioClip(string audioPath, UnityAction<AudioClip> callback)
        {
            var audioType = Path.GetExtension(audioPath) switch
            {
                ".wav" => AudioType.WAV,
                ".aif" => AudioType.AIFF,
                ".aiff" => AudioType.AIFF,
                ".mp2" => AudioType.MPEG,
                ".mp3" => AudioType.MPEG,
                ".ogg" => AudioType.OGGVORBIS,
                _ => AudioType.UNKNOWN
            };

            using var www = UnityWebRequestMultimedia.GetAudioClip(audioPath, audioType);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error + " / " + audioPath);
            }
            else
            {
                callback?.Invoke(DownloadHandlerAudioClip.GetContent(www));
            }
        }

        public static IEnumerator GetTexture(string imgPath, UnityAction<Texture2D> callback)
        {
            using var www = UnityWebRequestTexture.GetTexture(imgPath);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error + " / " + imgPath);
            }
            else
            {
                callback.Invoke(DownloadHandlerTexture.GetContent(www));
            }
        }
    }
}


[Serializable]
public class SerializableData
{
    public int version;
}

[Serializable]
public class SerializableDesc : SerializableData
{
    public string name;
    public string artist;
    public string genre;

    public double bpm;
    public List<SerializableBpm> bpms;

    public string musicPath;
    public string previewImgPath;
    public string smallImgPath;
    public string imgPath;
    public string mvPath;
}

[Serializable]
public class SerializablePattern : SerializableData
{
    public int line;
    public int level;
    public int diff;

    public List<SerializableNote> notes;
    public List<SerializableLongNote> longNotes;
}

[Serializable]
public class SerializableNote
{
    public int line;
    public double beat;

    public int CompareTo(SerializableNote other)
    {
        return beat.CompareTo(other.beat);
    }
}

[Serializable]
public class SerializableLongNote : SerializableNote
{
    public double length;
}

[Serializable]
public class SerializableBpm
{
    public double beat;
    public double bpm;
}