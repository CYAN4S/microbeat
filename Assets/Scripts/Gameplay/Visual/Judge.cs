using Core;
using SO;
using UnityEngine;

namespace Gameplay.Visual
{
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

        private void AnimateJudge(Judges value)
        {
            animator.SetTrigger(Const.JUDGE_NAME[(int) value]);
        }
    }
}