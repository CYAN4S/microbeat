using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LISystem : MonoBehaviour
{
    public DirectoryInfo dir;
    public SerializableDesc desc;
    public SerializablePattern pattern;

    #region INSPECTOR

    public Text title;
    public Text info;
    public Text level;

    #endregion
}