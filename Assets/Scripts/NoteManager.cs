using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    #region INSPECTOR
    public GameObject sheet;
    public GameObject notesParent;
    #endregion

    public static NoteManager Instance { get; private set; }

    private List<List<GameObject>> noteObjectsByLine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    private void LateUpdate()
    {

    }
}
