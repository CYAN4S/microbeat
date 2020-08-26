﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LISystem : MonoBehaviour
{
    #region INSPECTOR
    public Text title;
    public Text info;
    public Text level;
    #endregion

    public DirectoryInfo dir;
    public SerializableDesc desc;
    public SerializableSheet sheet;
}