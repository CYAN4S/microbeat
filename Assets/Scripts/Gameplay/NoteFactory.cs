using System;
using System.Collections;
using System.Collections.Generic;
using FileIO;
using Gameplay.Visual;
using Input;
using SO;
using SO.NormalChannel;
using SO.ParticularChannel;
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
                var noteSystem = CreateNote(item);
                player.EndTime = Math.Max(player.EndTime, noteSystem.time);
                sortReady[item.line].Add(noteSystem);
            }

            foreach (var item in pattern.longNotes)
            {
                var longNoteSystem = CreateLongNote(item);
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

        private NoteSystem CreateNote(SerializableNote item)
        {
            var target = line switch
            {
                4 => item.line == 1 || item.line == 2 ? skins.noteSet[note.value].notePrefab : skins.noteSet[note.value].notePrefabA,
                5 => item.line == 1 || item.line == 3 ? skins.noteSet[note.value].notePrefab : skins.noteSet[note.value].notePrefabA,
                6 => item.line == 1 || item.line == 4 ? skins.noteSet[note.value].notePrefab : skins.noteSet[note.value].notePrefabA,
                8 => item.line == 1 || item.line == 4 ? skins.noteSet[note.value].notePrefab : skins.noteSet[note.value].notePrefabA,
                _ => null
            };
            var noteSystem = Instantiate(target, noteContainer);
            noteSystem.OnGenerate(item, skinSystem.xPositions[item.line], skinSystem.scale);

            return noteSystem;
        }

        private LongNoteSystem CreateLongNote(SerializableLongNote item)
        {
            var target = line switch
            {
                4 => ((item.line == 1 || item.line == 2) ? skins.noteSet[note.value].longNotePrefab : skins.noteSet[note.value].longNotePrefabA),
                5 => ((item.line == 1 || item.line == 3) ? skins.noteSet[note.value].longNotePrefab : skins.noteSet[note.value].longNotePrefabA),
                6 => ((item.line == 1 || item.line == 4) ? skins.noteSet[note.value].longNotePrefab : skins.noteSet[note.value].longNotePrefabA),
                8 => ((item.line == 1 || item.line == 4) ? skins.noteSet[note.value].longNotePrefab : skins.noteSet[note.value].longNotePrefabA),
                _ => null
            };
            var longNoteSystem = Instantiate(target, noteContainer);
            longNoteSystem.OnGenerate(item, skinSystem.xPositions[item.line], skinSystem.scale);

            return longNoteSystem;
        }
    }
}