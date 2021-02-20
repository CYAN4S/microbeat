using SO;
using UnityEngine;

public class GrooveLight : MonoBehaviour
{
    [SerializeField] private PlayerSO player;
    [SerializeField] private Animator animator;
    private static readonly int Begin = Animator.StringToHash("Begin");
    private static readonly int End = Animator.StringToHash("End");
    private static readonly int Bpm = Animator.StringToHash("BPM");

    private void OnEnable()
    {
        player.ZeroEvent += StartGroove;
        player.GameEndEvent += StopGroove;
        player.BpmChangeEvent += Change;
    }

    private void OnDisable()
    {
        player.ZeroEvent -= StartGroove;
        player.GameEndEvent -= StopGroove;
        player.BpmChangeEvent -= Change;
    }

    private void StartGroove()
    {
        animator.SetTrigger(Begin);
    }

    private void StopGroove()
    {
        animator.SetTrigger(End);
    }

    private void Change()
    {
        animator.SetFloat(Bpm, (float) player.CurrentBpm / 60f);
    }
}