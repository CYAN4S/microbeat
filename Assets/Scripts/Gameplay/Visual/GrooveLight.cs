using System;
using SO;
using UnityEngine;

namespace Gameplay.Visual
{
    public class GrooveLight : MonoBehaviour
    {
        [SerializeField] private PlayerSO player;
        [SerializeField] private Animator animator;
        [SerializeField] private float multiply;

        private static readonly int Begin = Animator.StringToHash("Begin");

        // private static readonly int End = Animator.StringToHash("End");
        private static readonly int Bpm = Animator.StringToHash("BPM");

        private void OnEnable()
        {
            player.GameStartEvent += StartGroove;
            player.GameResumeEvent += StartGroove;
            player.GamePauseEvent += PauseGroove;
            player.BpmChangeEvent += Change;
        }

        private void OnDisable()
        {
            player.GameStartEvent -= StartGroove;
            player.GameResumeEvent -= StartGroove;
            player.GamePauseEvent -= PauseGroove;
            player.BpmChangeEvent -= Change;
        }

        private void StartGroove()
        {
            animator.speed = (float) player.CurrentBpm / 60f;
            animator.Play("Groove", 0, (float) Math.Abs(player.CurrentBeat % 1));
        }

        private void PauseGroove()
        {
            animator.speed = 0;
        }

        private void Change()
        {
            animator.speed = (float) player.CurrentBpm / 60f;
        }
    }
}