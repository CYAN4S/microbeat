using System;
using System.Runtime.CompilerServices;
using Core;
using FileIO;
using SO;
using SO.ParticularChannel;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class NoteSystem : MonoBehaviour, IComparable<NoteSystem>
    {
        public float time;

        [SerializeField] protected PlayerSO player;
        // [SerializeField] protected SkinSelectionSO skin;

        protected RectTransform rt;
        protected Image image;
        
        public int Line { get; protected set; }
        public double Beat { get; protected set; }

        protected void Awake()
        {
            rt = GetComponent<RectTransform>();
            image = GetComponent<Image>();
        }

        protected void LateUpdate()
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
            
            //image.sprite = Line == 1 || Line == 2 ? skin.noteSkin.whiteNote : skin.noteSkin.blueNote;
            
        }

        protected void Move()
        {
            transform.localPosition = new Vector3(transform.localPosition.x, GetCurrentYPos());
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