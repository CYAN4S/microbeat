using Input;
using UnityEngine;

public class PressEffect : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private int playKeyNumber;
    [SerializeField] private Animator[] animators;

    private void OnEnable()
    {
        inputReader.playKeyDownEvent += AnimateActivation;
        inputReader.playKeyUpEvent += AnimateDeactivation;
    }

    private void OnDisable()
    {
        inputReader.playKeyDownEvent -= AnimateActivation;
        inputReader.playKeyUpEvent -= AnimateDeactivation;
    }

    private void AnimateActivation(int key)
    {
        if (playKeyNumber != key) return;

        foreach (var animator in animators) animator.SetTrigger("Activate");
    }

    private void AnimateDeactivation(int key)
    {
        if (playKeyNumber != key) return;

        foreach (var animator in animators) animator.SetTrigger("Deactivate");
    }
}