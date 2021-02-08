using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UI;

public class PatternListContent : MonoBehaviour
{
    public Text text;
    
    public void SetValue(int line, int level, int diff)
    {
        text.text = $"{line}K\n{Const.PATTERN[diff]}\n{level}";
    }
}
