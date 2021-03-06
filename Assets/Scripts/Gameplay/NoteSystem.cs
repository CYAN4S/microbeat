﻿using System;
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
        protected int line;
        protected double beat;

        protected void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        protected void LateUpdate()
        {
            if (!player.IsWorking) return;

            Move();
        }

        public int CompareTo(NoteSystem other)
        {
            return beat.CompareTo(other.beat);
        }

        public void OnGenerate(SerializableNote data, float xPos, float scale)
        {
            line = data.line;
            beat = data.beat;
            time = player.Meta.GetTime(beat);
            transform.localPosition = new Vector3(xPos, 0);
            transform.localScale = new Vector3(scale, scale, 1);
        }

        protected void Move()
        {
            transform.localPosition = new Vector3(transform.localPosition.x, GetCurrentYPos());
        }

        protected float GetCurrentYPos()
        {
            return GetYPos(beat);
        }

        protected float GetYPos(double beat)
        {
            return (float) ((beat - player.CurrentBeat) * (float) player.ScrollSpeed * 36000f /
                            player.Meta.std);
        }
    }
}