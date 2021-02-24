using Input;
using UnityEngine;

namespace Gameplay.Visual
{
    public class PressEffect : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private int playKeyNumber;
        [SerializeField] private Animator[] animators;

        private void OnEnable()
        {
            inputReader.PlayKeyDownEvent += AnimateActivation;
            inputReader.PlayKeyUpEvent += AnimateDeactivation;
        }

        private void OnDisable()
        {
            inputReader.PlayKeyDownEvent -= AnimateActivation;
            inputReader.PlayKeyUpEvent -= AnimateDeactivation;
        }

        private void AnimateActivation(int key)
        {
            if (playKeyNumber != key) return;
            foreach (var animator in animators)
            {
                animator.Play("Activate", 0, 0);
            }
        }

        private void AnimateDeactivation(int key)
        {
            if (playKeyNumber != key) return;
            foreach (var animator in animators)
            {
                animator.Play("Deactivate", 0, 0);
            }
        }
    }
}