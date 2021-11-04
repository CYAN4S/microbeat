using SO;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ResumeAnimation : MonoBehaviour
{
    [SerializeField] private PlayerSO player;
    [SerializeField] private string stateName;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        player.GameResumeEvent += PlayAnimation;
    }

    private void OnDisable()
    {
        player.GameResumeEvent -= PlayAnimation;
    }

    private void PlayAnimation()
    {
        animator.Play(stateName);
    }
}
