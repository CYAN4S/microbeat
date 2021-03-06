using System;
using System.Collections;
using System.Collections.Generic;
using FileIO;
using Gameplay.Visual;
using Input;
using SO;
using UnityEngine;

namespace Gameplay
{
    public class NoteFactory : MonoBehaviour
    {
        [Header("Requirement")]

        [SerializeField] private PlayerSO player;

        [Header("Note Settings")]

        [SerializeField] private NoteSystem notePrefab;
        [SerializeField] private NoteSystem notePrefabA;
        [SerializeField] private LongNoteSystem longNotePrefab;
        [SerializeField] private LongNoteSystem longNotePrefabA;
        [SerializeField] private LongNoteSystem bumperNotePrefab;
        [SerializeField] private LongNoteSystem bumperNotePrefabA;

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
                4 => item.line == 0 || item.line == 3 ? notePrefab : notePrefabA,
                5 => item.line == 1 || item.line == 3 ? notePrefabA : notePrefab,
                6 => item.line == 1 || item.line == 4 ? notePrefabA : notePrefab,
                8 => item.line == 1 || item.line == 4 ? notePrefabA : notePrefab,
                _ => null
            };
            var noteSystem = Instantiate(target, noteContainer);
            noteSystem.SetFromData(item);

            noteSystem.transform.localPosition = new Vector3(skinSystem.xPositions[item.line], 0);
            noteSystem.time = player.Meta.GetTime(item.beat);
            noteSystem.transform.localScale = new Vector3(skinSystem.scale, skinSystem.scale, 1);

            return noteSystem;
        }

        private LongNoteSystem CreateLongNote(SerializableLongNote item)
        {
            var target = line switch
            {
                4 => ((item.line == 0 || item.line == 3) ? longNotePrefab : longNotePrefabA),
                5 => ((item.line == 1 || item.line == 3) ? longNotePrefabA : longNotePrefab),
                6 => ((item.line == 1 || item.line == 4) ? longNotePrefabA : longNotePrefab),
                8 => ((item.line == 1 || item.line == 4) ? longNotePrefabA : longNotePrefab),
                _ => null
            };
            var longNoteSystem = Instantiate(target, noteContainer);
            longNoteSystem.SetFromData(item);

            longNoteSystem.time = player.Meta.GetTime(item.beat);
            longNoteSystem.endTime = player.Meta.GetTime(item.beat + item.length);
            longNoteSystem.transform.localPosition = new Vector3(skinSystem.xPositions[item.line], 0);
            longNoteSystem.SetScale(skinSystem.scale);

            return longNoteSystem;
        }
    }
}
