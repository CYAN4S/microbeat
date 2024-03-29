using System;
using System.Collections.Generic;
using Core.SO.NormalChannel;
using Gameplay.Visual;
using SO;
using UnityEngine;

namespace Gameplay
{
    public class NoteFactory : MonoBehaviour
    {
        [Header("Requirement")]
        [SerializeField] private PlayerSO player;
        [SerializeField] private SkinCollection skins;

        [Header("Channel to get values from previous scene")]
        [SerializeField] private IntEventChannelSO note;

        private SkinSystem skinSystem;
        private Transform noteContainer;
        private int line;

        public List<Queue<NoteSystem>> PrepareNotes(SerializablePattern pattern, SkinSystem skin)
        {
            var noteQueues = new List<Queue<NoteSystem>>();
            var sortReady = new List<List<NoteSystem>>();

            line = pattern.line;
            skinSystem = skin;
            noteContainer = skin.noteContainer;

            player.EndTime = 3f;

            for (var i = 0; i < pattern.line; i++)
            {
                sortReady.Add(new List<NoteSystem>());
            }

            foreach (var item in pattern.notes)
            {
                var noteSystem = CreateNote(item.line, item.beat);
                player.EndTime = Math.Max(player.EndTime, noteSystem.time);
                sortReady[item.line].Add(noteSystem);
            }

            foreach (var item in pattern.longNotes)
            {
                var longNoteSystem = CreateLongNote(item.line, item.beat, item.length);
                player.EndTime = Math.Max(player.EndTime, longNoteSystem.endTime);
                sortReady[item.line].Add(longNoteSystem);
            }

            foreach (var item in sortReady)
            {
                item.Sort();
                noteQueues.Add(new Queue<NoteSystem>(item));
            }

            player.EndTime += 5f;

            return noteQueues;
        }

        private NoteSystem CreateNote(int noteline, double beat)
        {
            var target = line switch
            {
                4 => (noteline == 1 || noteline == 2) ? skins.noteSet[note.value].notePrefab : skins.noteSet[note.value].notePrefabA,
                5 => (noteline == 1 || noteline == 3) ? skins.noteSet[note.value].notePrefab : skins.noteSet[note.value].notePrefabA,
                6 => (noteline == 1 || noteline == 4) ? skins.noteSet[note.value].notePrefab : skins.noteSet[note.value].notePrefabA,
                8 => (noteline == 1 || noteline == 4) ? skins.noteSet[note.value].notePrefab : skins.noteSet[note.value].notePrefabA,
                _ => null
            };
            var noteSystem = Instantiate(target, noteContainer);
            noteSystem.OnGenerate(noteline, beat, skinSystem.xPositions[noteline], skinSystem.scale);

            return noteSystem;
        }

        private LongNoteSystem CreateLongNote(int noteline, double beat, double length)
        {
            var target = line switch
            {
                4 => ((noteline == 1 || noteline == 2) ? skins.noteSet[note.value].longNotePrefab : skins.noteSet[note.value].longNotePrefabA),
                5 => ((noteline == 1 || noteline == 3) ? skins.noteSet[note.value].longNotePrefab : skins.noteSet[note.value].longNotePrefabA),
                6 => ((noteline == 1 || noteline == 4) ? skins.noteSet[note.value].longNotePrefab : skins.noteSet[note.value].longNotePrefabA),
                8 => ((noteline == 1 || noteline == 4) ? skins.noteSet[note.value].longNotePrefab : skins.noteSet[note.value].longNotePrefabA),
                _ => null
            };
            var longNoteSystem = Instantiate(target, noteContainer);
            longNoteSystem.OnGenerate(noteline, beat, length, skinSystem.xPositions[noteline], skinSystem.scale);

            return longNoteSystem;
        }
    }
}