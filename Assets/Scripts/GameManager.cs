using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public AudioSource BackgroundMusicSource;
        public Text TestText;
        public Text TestText2;
        public GameObject NoteContainer;
        public GameObject NotePrefab;
        public TestKeyInput InputManager;

        public float NoteSpeed = 1;

        //public Queue<float> Notes = new Queue<float>();
        public List<ShortNote> Notes = new List<ShortNote>();

        public float sync = 0;
        private int n = 0;

        private const float JUDGE = 0.033f;

        // Use this for initialization
        private void Start()
        {
            var ran = new Random();
            foreach (var a in Enumerable.Range(2, 100))
            {
                foreach (var b in Enumerable.Range(1, 4))
                {
                    var CurNoteGO = Instantiate(NotePrefab, NoteContainer.transform);
                    var CurNote = CurNoteGO.GetComponent<ShortNote>();

                    CurNote.Timing = GetTime(a, b, 4, 143);
                    CurNote.Position = ran.Next(4)+1;
                    CurNote.Width = 1;

                    Notes.Add(CurNote);

                    CurNoteGO.transform.position = new Vector2(
                        (CurNote.Position - 1) * 0.5f - 4 + CurNote.Width / 4f,
                        CurNote.Timing * NoteSpeed
                    );

                    CurNoteGO.transform.localScale = new Vector3(CurNote.Width / 2f, 0.1f, 1);
                }
            }
        }

        private static float GetTime(int bar, int beat, int beatSplit, int bpm)
        {
            var res = 240 * ((double)(beat - 1) / beatSplit + (bar - 1)) / bpm;
            return (float)(res);
        }

        // Update is called once per frame
        private void Update ()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                BackgroundMusicSource.Play();

            if (!BackgroundMusicSource.isPlaying)
                return;

            NoteContainer.transform.position = new Vector2(0, -(BackgroundMusicSource.time * NoteSpeed + 4));

            MissCheck();
            
            JudgeNote();
        }

        private void JudgeNote()
        {
            var nowTime = BackgroundMusicSource.time;

            for (var i = 1; i <= 16; i++)
            {
                if (!InputManager.IsPressed(i)) continue;
                
                var cnt = 0;
                while (true)
                {
                    if (Notes[cnt++].Timing + sync - nowTime > JUDGE * 4) break;

                    var curNote = Notes[cnt - 1];

                    Debug.Log(curNote.Position + " / " + curNote.Width);

                    if (curNote.Position <= i && curNote.Position + curNote.Width - 1 >= i)
                    {
                        if (Math.Abs(nowTime - curNote.Timing + sync) < JUDGE)
                        {
                            TestText2.text = n++ + "\t" + "JUSTICE";
                        }
                        else if (Math.Abs(nowTime - curNote.Timing + sync) < JUDGE * 2.5)
                        {
                            TestText2.text = n++ + "\t" + "PERFECT";
                        }
                        else if (Math.Abs(nowTime - curNote.Timing + sync) < JUDGE * 4)
                        {
                            TestText2.text = n++ + "\t" + "ATTACK";
                        }
                        else if (Math.Abs(nowTime - curNote.Timing + sync) < JUDGE * 5)
                        {
                            TestText2.text = n++ + "\t" + "MISS";
                        }

                        Notes.RemoveAt(cnt-- - 1);
                        Destroy(curNote.gameObject);
                    }
                }
            }
        }

        private void MissCheck()
        {
            while (BackgroundMusicSource.time - (Notes.First().Timing + sync) > JUDGE * 4)
            {
                TestText2.text = n++ + "\t" + "MISS";
                Destroy(Notes.First().gameObject);
                Notes.RemoveAt(0);
            }
        }

        /*
        private void JudgeNote()
        {
            var nowTime = BackgroundMusicSource.time;
            if (Input.GetKeyDown(KeyCode.RightArrow) && Notes.Count > 0)
            {
                //var NextTiming = Notes.First;


                if (Math.Abs(nowTime - NextTiming + sync) < JUDGE)
                {
                    TestText2.text = n++ + "\t" + "JUSTICE";
                }
                else if (Math.Abs(nowTime - NextTiming + sync) < JUDGE * 2.5)
                {
                    TestText2.text = n++ + "\t" + "PERFECT";
                }
                else if (Math.Abs(nowTime - NextTiming + sync) < JUDGE * 4)
                {
                    TestText2.text = n++ + "\t" + "ATTACK";
                }
                else if (Math.Abs(nowTime - NextTiming + sync) < JUDGE * 5)
                {
                    TestText2.text = n++ + "\t" + "MISS";
                }
                else
                {
                    TestText2.text = n++ + "\t" + "?";
                    return;
                }

                // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                TestText.text = (BackgroundMusicSource.time - NextTiming + sync).ToString();
                Notes.Dequeue();
            }
            else
            {
                if (Notes.Count <= 0 || !(BackgroundMusicSource.time - Notes.Peek() + sync > JUDGE * 5)) return;

                TestText2.text = n++ + "\t" + "MISS";
                Notes.Dequeue();
            }
        }
        */
    }
}
