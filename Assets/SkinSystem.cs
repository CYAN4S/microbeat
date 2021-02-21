using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinSystem : MonoBehaviour
{
    [Header("Note Settings")]
    [SerializeField] public Transform noteContainer;

    [Header("Note X Position by line count")] [SerializeField]
    public int lineCount;

    [SerializeField] public float[] xPositions;
    [SerializeField] public float scale;

}