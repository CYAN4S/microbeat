using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PatternListContent : MonoBehaviour
    {
        public Text text;
        public Text lv;

        public void SetValue(int line, int level, int diff)
        {
            text.text = $"{line}K {Const.PATTERN[diff]}";
            lv.text = $"{level}";
        }
    }
}