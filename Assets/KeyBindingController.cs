using System;
using System.Collections;
using System.Collections.Generic;
using FileIO;
using UnityEngine;

public class KeyBindingController : MonoBehaviour
{
    [SerializeField] private KeyField[] speedKeys;
    [SerializeField] private KeyField[] playKeys;

    private KeyBinding keyBinding;

    private int currentKey;
    private KeyBinding.KeyPair currentPair;

    private void Awake()
    {
        keyBinding = FileExplorer.FromFile<KeyBinding>(KeyBinding.Path) ?? KeyBinding.Default();

        ResetValue(4);
    }

    public void OnDialogOpen()
    {
    }

    public void OnDialogClose()
    {
        FileExplorer.ToFile(keyBinding, KeyBinding.Path);
    }

    public void OnDropdownValueChange(int value)
    {
        ResetValue(value switch
        {
            0 => 4, 1 => 5, 2 => 6, 3 => 8, _ => -1
        });
    }

    private void ResetValue(int key)
    {
        currentKey = key;
        currentPair = keyBinding[currentKey];

        for (var i = 0; i < 4; i++)
        {
            speedKeys[i].SetValue(currentPair.speedKeys[i]);
        }

        for (var i = 0; i < currentPair.playKeys.Length; i++)
        {
            playKeys[i].SetValue(currentPair.playKeys[i]);
        }

        for (var i = currentPair.playKeys.Length; i < 8; i++)
        {
            playKeys[i].RemoveValue();
        }
    }
}