using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class NoteManager : MonoBehaviour
{
    #region INSPECTOR

    public Transform notesParent;
    public GameObject notePrefab;

    #endregion

    public static NoteManager Instance { get; private set; }

    private List<List<NoteSystem>> noteSystemsByLine;
    private List<Queue<NoteSystem>> noteSystemQs;

    private void Awake()
    {
        Instance = this;

        noteSystemsByLine = new List<List<NoteSystem>>();
        noteSystemQs = new List<Queue<NoteSystem>>();
        for (int i = 0; i < 4; i++)
        {
            noteSystemsByLine.Add(new List<NoteSystem>());

        }

        GameManager.OnCallSheet += PrepareNotes;
        GameManager.OnCallSheet += () => { InputManager.OnPlayKeyDown += JudgePlayKeyDown; };
    }

    private void PrepareNotes()
    {
        foreach (Note item in GameManager.CurrentSheet.notes)
        {
            var noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();
            noteSystemsByLine[item.line].Add(noteSystem);
            noteSystem.note = item;
            noteSystem.time = (float)(item.beat * (1f / GameManager.CurrentSheet.bpm) * 60f);
        }

        foreach (LongNote item in GameManager.CurrentSheet.longNotes)
        {
            var noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();
            noteSystemsByLine[item.line].Add(noteSystem);
            noteSystem.note = item;
            noteSystem.time = (float)(item.beat * GameManager.CurrentSheet.bpm * (1f / 60f));
        }

        foreach (var item in noteSystemsByLine)
        {
            item.Sort();
            noteSystemQs.Add(new Queue<NoteSystem>(item));
        }

    }

    private void Update()
    {

    }

    private void JudgePlayKeyDown(int key)
    {
        if (noteSystemQs[key].Count == 0)
            return;

        NoteSystem target = noteSystemQs[key].Peek();
        float gap = target.time - GameManager.CurrentTime;

        if (gap > GameManager.JUDGESTD[3]) // DONT CARE
        {
            return;
        }

        if (Math.Abs(gap) > GameManager.JUDGESTD[2])
        {

        }
        else if (true)
        {

        }
    }

}
