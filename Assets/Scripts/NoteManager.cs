using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    #region INSPECTOR

    public Sheet sheet;
    public Transform notesParent;
    public GameObject notePrefab;
    public float[] linePositions;

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
    }

    private void Start()
    {
        foreach (Note item in sheet.notes)
        {
            var noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();
            noteSystemsByLine[item.line].Add(noteSystem);
            noteSystem.noteData = item;
        }

        foreach (LongNote item in sheet.longNotes)
        {
            var noteSystem = Instantiate(notePrefab, notesParent).GetComponent<NoteSystem>();
            noteSystemsByLine[item.line].Add(noteSystem);
            noteSystem.noteData = item;
        }

        foreach (var item in noteSystemsByLine)
        {
            item.Sort();
        }

        foreach (var item in noteSystemsByLine)
        {
            foreach (var noteSystem in item)
            {
                Note note = noteSystem.noteData;
                noteSystem.gameObject.transform.localPosition = new Vector3(linePositions[note.line], (float)note.timing * 400);
            }
        }
    }

    private void Update()
    {

    }

}
