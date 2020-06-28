using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    #region INSPECTOR

    public Transform notesParent;
    public GameObject notePrefab;

    #endregion

    public static NoteManager Instance { get; private set; }

    private List<List<NoteSystem>> noteSystemsByLine;

    private void Awake()
    {
        Instance = this;

        noteSystemsByLine = new List<List<NoteSystem>>();
        for (int i = 0; i < 4; i++)
        {
            noteSystemsByLine.Add(new List<NoteSystem>());
        }

        GameManager.OnCallSheet += PrepareNotes;
    }

    private void PrepareNotes()
    {
        foreach (Note item in GameManager.CurrentSheet.notes)
        {
            var noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();
            noteSystemsByLine[item.line].Add(noteSystem);
            noteSystem.note = item;
        }

        foreach (LongNote item in GameManager.CurrentSheet.longNotes)
        {
            var noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();
            noteSystemsByLine[item.line].Add(noteSystem);
            noteSystem.note = item;
        }

        foreach (var item in noteSystemsByLine)
        {
            item.Sort();
        }

        foreach (var item in noteSystemsByLine)
        {
            foreach (var noteSystem in item)
            {
                Note note = noteSystem.note;
            }
        }
    }

    private void Update()
    {

    }

}
