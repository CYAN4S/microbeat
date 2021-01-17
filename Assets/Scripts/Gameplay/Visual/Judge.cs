using Events;
using UnityEngine;

public class Judge : MonoBehaviour
{
    [SerializeField] private PlayerSO player;
    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        player.JudgeEvent += AnimateJudge;
    }

    private void OnDisable()
    {
        player.JudgeEvent -= AnimateJudge;
    }

    private void AnimateJudge(JUDGES value)
    {
        animator.SetTrigger(CONST.JUDGE_NAME[(int) value]);
    }
}