using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NongameUIManager : MonoBehaviour
{
    public GameObject selection, pause, result;
    public static NongameUIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        selection.SetActive(true);
    }

    public void DisplayResult()
    {
        result.SetActive(true);
    }
}
