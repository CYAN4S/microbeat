using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FileIO;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyBindingController : MonoBehaviour
{
    [SerializeField] private KeyField[] speedKeys;
    [SerializeField] private KeyField[] playKeys;

    // private KeyBinding keyBinding;
    private Binding binding;

    private int currentKey;
    private Binding.BindingByMode currentPair;

    private void Awake()
    {
        for (var index = 0; index < speedKeys.Length; index++)
        {
            var keyField = speedKeys[index];
            keyField.targetArray = speedKeys;
            keyField.targetIndex = index;
            keyField.OnValueChangeByInput += key => { OnKeyFieldValueChanged(key, keyField); };
        }

        for (var index = 0; index < playKeys.Length; index++)
        {
            var keyField = playKeys[index];
            keyField.targetArray = playKeys;
            keyField.targetIndex = index;
            keyField.OnValueChangeByInput += key => { OnKeyFieldValueChanged(key, keyField); };
        }
    }

    public void OnKeyFieldValueChanged(Key key, KeyField target)
    {
        if (target.targetArray == speedKeys)
        {
            // keyBinding[currentKey].speedKeys[target.targetIndex] = key;
            binding.Dict[currentKey].Speed[target.targetIndex] = key;
        }
        else if (target.targetArray == playKeys)
        {
            // keyBinding[currentKey].playKeys[target.targetIndex] = key;
            binding.Dict[currentKey].Play[target.targetIndex] = key;
        }
        target.SetValue(key);
    }

    public void OnDialogOpen()
    {
        binding = FileExplorer.FromFile<Binding>(Binding.Path) ?? Binding.Default();
        ResetValue(4);
    }

    public void OnDialogClose()
    {
        FileExplorer.ToFile(binding, Binding.Path);
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
        currentPair = binding.Dict[currentKey];

        for (var i = 0; i < 4; i++)
        {
            speedKeys[i].SetValue(currentPair.Speed[i]);
        }

        for (var i = 0; i < currentPair.Play.Length; i++)
        {
            playKeys[i].SetValue(currentPair.Play[i]);
            playKeys[i].SetInteractable(true);
        }

        for (var i = currentPair.Play.Length; i < 8; i++)
        {
            playKeys[i].RemoveValue();
            playKeys[i].SetInteractable(false);
        }
    }
}