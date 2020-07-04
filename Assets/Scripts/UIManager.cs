using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject[] pressEffectObjects;
    public GameObject[] pressButtonObjects;
    public Text speedText, scoreText;
    public Text timeText;

    private void Awake()
    {
        InputManager.OnPlayKeyDown += n =>
        {
            pressEffectObjects[n].SetActive(true);
            pressButtonObjects[n].SetActive(true);
        };

        InputManager.OnPlayKeyUp += n =>
        {
            pressEffectObjects[n].SetActive(false);
            pressButtonObjects[n].SetActive(false);
        };
    }

    private void LateUpdate()
    {
        if (!GameManager.IsWorking)
        {
            return;
        }

        speedText.text = GameManager.ScrollSpeed.ToString("F2");
        timeText.text = GameManager.CurrentTime.ToString("F3");
    }
}
