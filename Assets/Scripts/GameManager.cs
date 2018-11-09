﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ruwa.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public AudioSource BackgroundMusicSource;
        public Text TestText;
        public Text TestText2;
        public TestKeyInput InputManager;

        public GameObject NoteContainer;
        public GameObject TabPrefab;
        public GameObject HoldPrefab;

        public float NoteSpeed = 1;

        public List<Note> Notes = new List<Note>();
        public List<Holdable> HoldingNotes = new List<Holdable>();
        public List<Ruwa.Objects.Note> NoteDatas = new List<Ruwa.Objects.Note>();

        public float sync = 0;
        private int n = 0;

        private const float JUDGE = 0.033f;


        private void Start()
        {
            var ran = new System.Random();
            foreach (var a in Enumerable.Range(2, 100))
            {

                foreach (var b in Enumerable.Range(0, 2))
                {
                    var pos = ran.Next(4);
                    NoteDatas.Add(
                        new Ruwa.Objects.Hold()
                        {
                            BeginKeyframe = new Ruwa.Objects.Keyframe()
                            {
                                Bar = a,
                                CurBeat = b*2 + 1,
                                FullBeat = 4,
                                Position = pos * 2 + 1,
                                Size = 2
                            },
                            EndKeyframe = new Ruwa.Objects.Keyframe()
                            {
                                Bar = a,
                                CurBeat = b*2 + 3,
                                FullBeat = 4,
                                Position = pos * 2 + 1,
                                Size = 2
                            }
                        }
                        );
                }
            }

            foreach (var a in NoteDatas)
            {
                if (a is Ruwa.Objects.Tab)
                {
                    CreateNote((Ruwa.Objects.Tab)a);
                }
                else if (a is Ruwa.Objects.Hold)
                {
                    CreateNote((Ruwa.Objects.Hold)a);
                }
            }
        }

        private static float GetTime(int bar, int beat, int beatSplit, int bpm)
        {
            var res = 240 * ((double)(beat - 1) / beatSplit + (bar - 1)) / bpm;
            return (float)(res);
        }

        private void CreateNote(Ruwa.Objects.Tab note)
        {
            var CurNoteGO = Instantiate(TabPrefab, NoteContainer.transform);
            var CurNote = CurNoteGO.GetComponent<Tab>();

            CurNote.Time = GetTime(note.BeginKeyframe.Bar, note.BeginKeyframe.CurBeat, note.BeginKeyframe.FullBeat, 143);
            CurNote.Position = note.BeginKeyframe.Position;
            CurNote.Width = note.BeginKeyframe.Size;

            Notes.Add(CurNote);

            CurNoteGO.transform.position = new Vector2(
                (CurNote.Position - 1) * 0.5f - 4 + CurNote.Width / 4f,
                CurNote.Time * NoteSpeed
            );

            CurNoteGO.transform.localScale = new Vector3(CurNote.Width / 2f, 0.1f, 1);
        }

        private void CreateNote(Ruwa.Objects.Hold note)
        {
            var CurNoteGO = Instantiate(HoldPrefab, NoteContainer.transform);
            var CurNote = CurNoteGO.GetComponent<Hold>();

            CurNote.Time = GetTime(note.BeginKeyframe.Bar, note.BeginKeyframe.CurBeat, note.BeginKeyframe.FullBeat, 143);
            CurNote.EndTime = GetTime(note.EndKeyframe.Bar, note.EndKeyframe.CurBeat, note.EndKeyframe.FullBeat, 143);
            CurNote.Position = note.BeginKeyframe.Position;
            CurNote.Width = note.BeginKeyframe.Size;

            Notes.Add(CurNote);

            CurNoteGO.transform.position = new Vector2(
                (CurNote.Position - 1) * 0.5f - 4 + CurNote.Width / 4f,
                CurNote.Time * NoteSpeed + (CurNote.EndTime - CurNote.Time) * NoteSpeed / 2
            );

            CurNoteGO.transform.localScale = new Vector3(CurNote.Width / 2f, (CurNote.EndTime - CurNote.Time) * NoteSpeed, 1);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                BackgroundMusicSource.Play();
                StartCoroutine("HoldChecker");
            }

            if (!BackgroundMusicSource.isPlaying)
                return;

            NoteContainer.transform.position = new Vector2(0, -(BackgroundMusicSource.time * NoteSpeed + 4));

            MissCheck();
            HoldCheck();
            JudgeNote();
        }

        private void JudgeNote()
        {
            var nowTime = BackgroundMusicSource.time;

            for (var i = 1; i <= 16; i++)
            {
                if (!InputManager.IsPressed(i)) continue;

                var cnt = 0;
                while (Notes.Count > 0)
                {
                    if (Notes[cnt++].Time + sync - nowTime > JUDGE * 4) break;

                    if (Notes[cnt - 1] is Tab)
                    {
                        var curNote = Notes[cnt - 1];

                        Debug.Log(curNote.Position + " / " + curNote.Width);

                        if (curNote.Position <= i && curNote.Position + curNote.Width - 1 >= i)
                        {
                            if (Math.Abs(nowTime - curNote.Time + sync) < JUDGE)
                            {
                                TestText2.text = ++n + "\t" + "JUSTICE";
                            }
                            else if (Math.Abs(nowTime - curNote.Time + sync) < JUDGE * 2.5)
                            {
                                TestText2.text = ++n + "\t" + "PERFECT";
                            }
                            else if (Math.Abs(nowTime - curNote.Time + sync) < JUDGE * 4)
                            {
                                n = 0;
                                TestText2.text = n + "\t" + "ATTACK";
                            }
                            else if (Math.Abs(nowTime - curNote.Time + sync) < JUDGE * 5)
                            {
                                n = 0;
                                TestText2.text = n + "\t" + "MISS";
                            }

                            Notes.RemoveAt(cnt-- - 1);
                            Destroy(curNote.gameObject);
                        }

                    }
                    else if (Notes[cnt - 1] is Hold)
                    {
                        var curNote = Notes[cnt - 1];

                        Debug.Log(curNote.Position + " / " + curNote.Width);

                        if (curNote.Position > i || curNote.Position + curNote.Width - 1 < i) continue;

                        if (Math.Abs(nowTime - curNote.Time + sync) < JUDGE)
                        {
                            TestText2.text = ++n + "\t" + "JUSTICE";
                        }
                        else if (Math.Abs(nowTime - curNote.Time + sync) < JUDGE * 2.5)
                        {
                            TestText2.text = ++n + "\t" + "PERFECT";
                        }
                        else if (Math.Abs(nowTime - curNote.Time + sync) < JUDGE * 4)
                        {
                            n = 0;
                            TestText2.text = n + "\t" + "ATTACK";
                        }
                        else if (Math.Abs(nowTime - curNote.Time + sync) < JUDGE * 5)
                        {
                            n = 0;
                            TestText2.text = n + "\t" + "MISS";

                            Notes.RemoveAt(cnt-- - 1);
                            Destroy(curNote.gameObject);
                            continue;
                        }
                        
                        HoldingNotes.Add(curNote as Holdable);
                        Notes.RemoveAt(cnt-- - 1);
                    }
                }
            }
        }

        private void MissCheck()
        {
            while (Notes.Count > 0 && BackgroundMusicSource.time - (Notes[0].Time + sync) > JUDGE * 4)
            {
                n = 0;
                TestText2.text = n + "\t" + "MISS";
                Destroy(Notes[0].gameObject);
                Notes.RemoveAt(0);
            }

            for (var i = 0; i < HoldingNotes.Count; i++)
            {
                var curNote = HoldingNotes[i];

                if (curNote.EndTime <= BackgroundMusicSource.time)
                {
                    HoldingNotes.RemoveAt(i--);
                    Destroy(curNote.gameObject);
                }
            }
        }

        private void HoldCheck()
        {
            var dt = Time.fixedDeltaTime;

            for (var i = 0; i < HoldingNotes.Count; i++)
            {
                var curNote = HoldingNotes[i];
                curNote.Cooldown += dt;

                if (curNote.Cooldown < 0.3f) continue;

                curNote.Cooldown -= 0.4f;

                var isHolding = false;
                for (var j = curNote.Position; j < curNote.Position + curNote.Width; j++)
                {
                    if (!InputManager.IsPressing(j)) continue;

                    isHolding = true;
                    break;
                }

                if (!isHolding)
                {
                    n = 0;
                    TestText2.text = n + "\t" + "MISS";
                    HoldingNotes.RemoveAt(i--);
                    Destroy(curNote.gameObject);
                }
                else
                {
                    TestText2.text = ++n + "\t" + "JUSTICE";
                }
            }
        }
    }
}
