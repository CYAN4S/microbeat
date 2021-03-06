using System.Collections.Generic;
using Core;
using FileIO;
using UnityEditor;
using UnityEngine;

namespace Gameplay
{
    public class LongNoteSystem : NoteSystem
    {
        public float endTime;
        public double length;
        public Queue<double> ticks;
        public bool isIn;

        public bool pausedWhileIsIn;
        public double pausedBeat;
        public float pausedTime;

        [SerializeField] private Transform topPart;
        [SerializeField] private Transform bottomPart;

        private float endPos;
        private float startPos;

        protected new void Awake()
        {
            base.Awake();
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

        public void OnGenerate(SerializableLongNote data, float xPos, float scale)
        {
            line = data.line;
            beat = data.beat;
            length = data.length;
            for (var i = 0.25; i < length; i += 0.25) ticks.Enqueue(i);

            time = player.Meta.GetTime(beat);
            endTime = player.Meta.GetTime(beat + length);

            transform.localPosition = new Vector3(xPos, 0);
            SetScale(scale);
        }

        private void ChangeLength()
        {
            var l = isIn ? endPos : endPos - startPos;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, l);
        }

        protected new void Move()
        {
            transform.localPosition = new Vector3(transform.localPosition.x, (startPos + endPos) / 2f);
        }

        protected float GetCurrentEndYPos()
        {
            return GetYPos(beat + length);
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

        private void SetScale(float value)
        {
            transform.localScale = new Vector3(value, 1, 1);
            topPart.localScale = new Vector3(1, value, 1);
            bottomPart.localScale = new Vector3(1, value, 1);
        }
    }
}