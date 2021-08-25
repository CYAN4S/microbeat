using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using FileIO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyBindingController : MonoBehaviour
{
    [SerializeField] private KeyField[] speedKeys;
    [SerializeField] private KeyField[] playKeys;
    [SerializeField] private GameObject[] Info;

    private Binding binding;
    private int currentKey;
    private BindingByMode currentBindingByMode;

    private void Awake()
    {
        for (var index = 0; index < speedKeys.Length; index++)
        {
            var keyField = speedKeys[index];

            keyField.targetEnum = BindingEnum.Speed;
            keyField.targetIndex = index;

            keyField.OnValueChangeByInput += key => { OnKeyFieldValueChanged(key, keyField); };
        }

        for (var index = 0; index < playKeys.Length; index++)
        {
            var keyField = playKeys[index];

            keyField.targetEnum = BindingEnum.Play;
            keyField.targetIndex = index;

            keyField.OnValueChangeByInput += key => { OnKeyFieldValueChanged(key, keyField); };
        }
    }

    public void OnKeyFieldValueChanged(Key key, KeyField target)
    {
        var prev = binding[currentKey][(int) target.targetEnum][target.targetIndex];

        for (var i = 0; i < binding[currentKey].Count; i++)
        {
            var list = binding[currentKey][i];
            for (var index = 0; index < list.Count; index++)
            {
                if (list[index] != key) continue;

                list[index] = prev;
                ((BindingEnum) i switch
                {
                    BindingEnum.Speed => speedKeys,
                    BindingEnum.Play => playKeys,
                    _ => throw new ArgumentOutOfRangeException()
                })[index].SetValue(prev);
            }
        }
        binding[currentKey][(int) target.targetEnum][target.targetIndex] = key;
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
        ResetValue(value switch {0 => 4, 1 => 5, 2 => 6, 3 => 8, _ => -1});
    }

    private void ResetValue(int key)
    {
        currentKey = key;
        currentBindingByMode = binding[currentKey];

        for (var i = 0; i < 4; i++)
        {
            speedKeys[i].SetValue(currentBindingByMode.Speed[i]);
        }

        for (var i = 0; i < currentBindingByMode.Play.Count; i++)
        {
            playKeys[i].SetValue(currentBindingByMode.Play[i]);
            playKeys[i].SetInteractable(true);
        }

        for (var i = currentBindingByMode.Play.Count; i < playKeys.Length; i++)
        {
            playKeys[i].RemoveValue();
            playKeys[i].SetInteractable(false);
        }

        foreach (var o in Info)
        {
            o.SetActive(false);
        }

        Info[key switch {4 => 0, 5 => 1, 6 => 2, 8 => 3, _ => -1}].SetActive(true);
    }
}