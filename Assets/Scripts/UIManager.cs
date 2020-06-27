using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject[] pressEffectObjects;
    public Text speedText, scoreText;
    public Text timeText;


    private void Start()
    {
        InputManager.OnPlayKeyDown += n =>
        {
            pressEffectObjects[n].SetActive(true);
        };

        InputManager.OnPlayKeyUp += n =>
        {
            pressEffectObjects[n].SetActive(false);
        };
    }

    private void LateUpdate()
    {
        timeText.text = GameManager.CurrentTime.ToString("F3");
    }
}
