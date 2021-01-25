using System.IO;
using FileIO;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LISystem : MonoBehaviour
    {
        public DirectoryInfo dir;
        public SerializableDesc desc;
        public SerializablePattern pattern;

        public Text title;
        public Text info;
        public Text level;
    }
}