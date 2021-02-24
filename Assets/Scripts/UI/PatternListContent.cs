using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PatternListContent : MonoBehaviour
    {
        public Text text;

        public void SetValue(int line, int level, int diff)
        {
            text.text = $"{line}K\n{Const.PATTERN[diff]}\n{level}";
        }
    }
}