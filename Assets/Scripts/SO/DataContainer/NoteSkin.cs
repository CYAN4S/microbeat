using UnityEngine;

namespace SO.DataContainer
{
    [CreateAssetMenu(menuName = "Data Container/NoteSkin")]
    public class NoteSkin : ScriptableObject
    {
        [Header("Note")] public Sprite blueNote;
        public Sprite whiteNote;

        public Sprite blueLong;
        public Sprite whiteLong;

        public Animator blueAnimator;
        public Animator whiteAnimator;

        public float noteHeight;
    }
}