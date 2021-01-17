using Events;
using UnityEngine;

public class GrooveLight : MonoBehaviour
{
    [SerializeField] private PlayerSO player;
    [SerializeField] private Animator animator;

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
        animator.SetTrigger("Begin");
    }

    public void StopGroove()
    {
        animator.SetTrigger("End");
    }

    public void Change()
    {
        animator.SetFloat("BPM", (float) player.CurrentBpm / 60f);
    }
}