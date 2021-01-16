using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class NoteEffect : MonoBehaviour
{
    [SerializeField] private PlayerSO player;
    [SerializeField] private Animator animator;
    [SerializeField] private int line;

    private void OnEnable()
    {
        player.NoteEffectEvent += Animate;
    }

    private void OnDisable()
    {
        player.NoteEffectEvent -= Animate;
    }

    private void Animate(int value)
    {
        if (value == line) animator.SetTrigger("Effect");
    }
}
