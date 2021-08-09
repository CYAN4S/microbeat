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

        public List<Queue<NoteSystem>> PrepareNotes(SerializablePatternData pattern, SkinSystem skin)
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
                var noteSystem = CreateNote((int)item[0], item[1]);
                player.EndTime = Math.Max(player.EndTime, noteSystem.time);
                sortReady[(int)item[0]].Add(noteSystem);
            }

            foreach (var item in pattern.longNotes)
            {
                var longNoteSystem = CreateLongNote((int)item[0], item[1], item[2]);
                player.EndTime = Math.Max(player.EndTime, longNoteSystem.endTime);
                sortReady[(int)item[0]].Add(longNoteSystem);
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

        private LongNoteSystem CreateLongNote(int line, double beat, double length)
        {
            var target = line switch
            {
                4 => ((line == 1 || line == 2) ? skins.noteSet[note.value].longNotePrefab : skins.noteSet[note.value].longNotePrefabA),
                5 => ((line == 1 || line == 3) ? skins.noteSet[note.value].longNotePrefab : skins.noteSet[note.value].longNotePrefabA),
                6 => ((line == 1 || line == 4) ? skins.noteSet[note.value].longNotePrefab : skins.noteSet[note.value].longNotePrefabA),
                8 => ((line == 1 || line == 4) ? skins.noteSet[note.value].longNotePrefab : skins.noteSet[note.value].longNotePrefabA),
                _ => null
            };
            var longNoteSystem = Instantiate(target, noteContainer);
            longNoteSystem.OnGenerate(line, beat, length, skinSystem.xPositions[line], skinSystem.scale);

            return longNoteSystem;
        }
    }
}