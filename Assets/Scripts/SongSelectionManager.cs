using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongSelectionManager : MonoBehaviour
{
    public GameObject main;

    private void Awake()
    {
        main.SetActive(true);
    }
}
