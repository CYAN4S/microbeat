using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private PlayerSO player;
    [SerializeField] private GameObject dialogue;

    private void OnEnable()
    {
        player.GamePauseEvent += OnPause;
        player.GameResumeEvent += OnResume;
    }
    
    private void OnDisable()
    {
        player.GamePauseEvent -= OnPause;
        player.GameResumeEvent -= OnResume;
    }

    private void OnPause()
    {
        dialogue.SetActive(true);
    }

    private void OnResume()
    {
        dialogue.SetActive(false);
    }
}
