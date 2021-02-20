using System;
using System.Collections;
using System.Collections.Generic;
using SO;
using UnityEngine;
using UnityEngine.UI;

public class GrooveMeter : MonoBehaviour
{
    [SerializeField] private PlayerSO player;
    [SerializeField] private Image image;

    private void Update()
    {
        image.fillAmount = player.GrooveMeter;
    }
}
