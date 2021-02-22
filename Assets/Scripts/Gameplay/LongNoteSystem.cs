using System.Collections.Generic;
using Core;
using FileIO;
using UnityEditor;
using UnityEngine;

namespace Gameplay
{
    public class LongNoteSystem : NoteSystem
    {
        // public const float noteHeight = 50f;
        public float endTime;
        public double length;
        public Queue<double> ticks;
        public bool isIn;

        public bool pausedWhileIsIn;
        public double pausedBeat;
        public float pausedTime;


        private float endPos;
        private float startPos;

        protected new void Awake()
        {
            base.Awake();
            // rt = GetComponent<RectTransform>();
            ticks = new Queue<double>();

            //GameManager.Instance.OnScrollSpeedChange += ChangeLength;
        }

        protected new void LateUpdate()
        {
            if (!player.IsWorking) return;

            GetPoses();
            Move();
            ChangeLength();
        }

        public void SetFromData(SerializableLongNote data)
        {
            Line = data.line;
            Beat = data.beat;
            length = data.length;
            for (var i = 0.25; i < length; i += 0.25) ticks.Enqueue(i);
            
            // image.sprite = Line == 1 || Line == 2 ? skin.noteSkin.whiteNote : skin.noteSkin.blueNote;
        }

        private void ChangeLength()
        {
            var l = isIn ? endPos : endPos - startPos;
            // l += skin.noteSkin.noteHeight;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, l);
        }

        protected new void Move()
        {
            transform.localPosition = new Vector3(transform.localPosition.x, (startPos + endPos) / 2f);
        }

        protected float GetCurrentEndYPos()
        {
            return GetYPos(Beat + length);
            // return (float) ((Beat + length - player.CurrentBeat) * (float) player.ScrollSpeed * 36000f / player.StdBpm);
        }

        protected void GetPoses()
        {
            if (pausedWhileIsIn)
                startPos = Mathf.Max(GetYPos(pausedBeat), 0);
            else if (isIn)
                startPos = 0;
            else
                startPos = GetCurrentYPos();

            endPos = GetCurrentEndYPos();
        }
    }
}