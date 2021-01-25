using System;
using Core;
using FileIO;
using SO;
using UnityEngine;

namespace Gameplay
{
    public class NoteSystem : MonoBehaviour, IComparable<NoteSystem>
    {
        public float time;

        [SerializeField] protected PlayerSO player;

        protected RectTransform rt;
        public int Line { get; protected set; }
        public double Beat { get; protected set; }

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            if (!player.IsWorking) return;

            Move();
        }

        public int CompareTo(NoteSystem other)
        {
            return Beat.CompareTo(other.Beat);
        }

        public void SetFromData(SerializableNote data)
        {
            Line = data.line;
            Beat = data.beat;
        }

        private void Move()
        {
            transform.localPosition = new Vector3(Const.LINE_X_POS[Line], GetCurrentYPos());
        }

        protected float GetCurrentYPos()
        {
            return GetYPos(Beat);
        }

        protected float GetYPos(double beat)
        {
            return (float) ((beat - player.CurrentBeat) * (float) player.ScrollSpeed * 36000f /
                            player.Meta.std);
        }
    }
}