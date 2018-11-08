﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ruwa.Objects;
using UnityEngine;
using UnityEngine.UI;
using GameObject = UnityEngine.GameObject;
using Random = System.Random;

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

        public float Sync = 0;
        private int ComboCount = 0;
        //private int n = 0;

        private const float Judge = 0.033f;
        private const int MaxLineCount = 16;
        private const string JusticeJudgeText = "JUSTICE";
        private const string PerfectJudgeText = "PERFECT";
        private const string AttackudgeText = "ATTACK";
        private const string MissJudgeText = "MISS";

        private void Start()
        {
            foreach (var a in NoteDatas)
            {
                if (a is Ruwa.Objects.Tab)
                {
                    CreateNote((Ruwa.Objects.Tab) a);
                }   
                else if (a is Ruwa.Objects.Hold)
                {
                    CreateNote((Ruwa.Objects.Hold) a);
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                BackgroundMusicSource.Play();

            if (!BackgroundMusicSource.isPlaying)
                return;

            NoteContainer.transform.position = new Vector2(0, -(BackgroundMusicSource.time * NoteSpeed + 4));

            MissCheck();
            JudgeNote();
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
            var CurNoteGO = Instantiate(TabPrefab, NoteContainer.transform);
            var CurNote = CurNoteGO.GetComponent<Hold>();

            CurNote.Time = GetTime(note.BeginKeyframe.Bar, note.BeginKeyframe.CurBeat, note.BeginKeyframe.FullBeat, 143);
            CurNote.EndTime = GetTime(note.EndKeyframe.Bar, note.EndKeyframe.CurBeat, note.EndKeyframe.FullBeat, 143);
            CurNote.Position = note.BeginKeyframe.Position;
            CurNote.Width = note.BeginKeyframe.Size;

            Notes.Add(CurNote);

            CurNoteGO.transform.position = new Vector2(
                (CurNote.Position - 1) * 0.5f - 4 + CurNote.Width / 4f,
                CurNote.Time * NoteSpeed
            );

            CurNoteGO.transform.localScale = new Vector3(CurNote.Width / 2f, 0.1f, 1);
        }

        private void JudgeNote()
        {
            var nowTime = BackgroundMusicSource.time;

            for (var line = 1; line <= MaxLineCount; line++)
            {
                if (!InputManager.IsPressed(line)) continue;
                
                var cnt = 0;
                while (Notes.Count > 0)
                {
                    if (Notes[cnt++].Time + Sync - nowTime > Judge * 4) break;

                    var curNote = Notes[cnt - 1];

                    Debug.Log(curNote.Position + " / " + curNote.Width);

                    if (curNote.Position <= line && curNote.Position + curNote.Width - 1 >= line)
                    {
                        var parallax = Math.Abs(nowTime - curNote.Time + Sync);
                        if (parallax < Judge)
                        {
                            TestText2.text = ++ComboCount + "\t" + JusticeJudgeText;
                        }
                        else if (parallax < Judge * 2.5)
                        {
                            TestText2.text = ++ComboCount + "\t" + PerfectJudgeText;
                        }

                        else if (parallax < Judge * 4)
                        {
                            ComboCount = 0;
                            TestText2.text = ComboCount + "\t" + AttackudgeText;
                        }
                        else if (parallax < Judge * 5)
                        {
                            ComboCount = 0;
                            TestText2.text = ComboCount + "\t" + MissJudgeText;
                        }

                        Notes.RemoveAt(cnt-- - 1);
                        Destroy(curNote.gameObject);
                    }
                }
            }
        }

        private void MissCheck()
        {
            while (Notes.Count > 0 && BackgroundMusicSource.time - (Notes.First().Time + Sync) > Judge * 4)
            {
                ComboCount = 0;
                TestText2.text = ComboCount + "\t" + MissJudgeText;
                Destroy(Notes.First().gameObject);
                Notes.RemoveAt(0);
            }
        }

        IEnumerator HoldChecker()
        {
            for (var j = 0; j < HoldingNotes.Count; j++)
            {
                var noteWidth = HoldingNotes[j].Width;
                var notePos = HoldingNotes[j].Position;

                bool res = false;
                for (int t = notePos; t <= noteWidth; t++)
                {
                    if (HoldingNotes[j] is Hold)
                    {
                        res = true;
                        break;
                    }
                    // 에어 홀드 처리는 요기서 해주세용~~~. :yes:
                }

                if (res)
                {
                    TestText2.text = ++ComboCount + "\t" + JusticeJudgeText;
                }
                else
                {
                    ComboCount = 0;
                    TestText2.text = ComboCount + "\t" + MissJudgeText;
                    Destroy(HoldingNotes[j]);
                    HoldingNotes.RemoveAt(j--);
                }
            }

            yield return new WaitForSeconds(0.5f);

            if (BackgroundMusicSource.isPlaying)
                StartCoroutine("HoldChecker");
        }
    }
}
