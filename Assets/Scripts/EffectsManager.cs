using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public GameObject[] pressEffectObjects;

    private void Start()
    {
        InputManager.OnPlayKeyDown += n => {
            pressEffectObjects[n].SetActive(true);
        };

        InputManager.OnPlayKeyUp += n => {
            pressEffectObjects[n].SetActive(false);
        };
    }
}
