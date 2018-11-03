using System.Collections.Generic;
using UnityEngine;

namespace Ruwa.Objects
{
    class Song
    {
        public Metadata SongMetadata { get; set; }
        public Sheet SongSheet { get; set; }
    }
    class Metadata
    {
        public string SongName { get; set; }
        public string SongComposer { get; set; }
        public string SheetComposer { get; set; }
        public List<BPMData> BPMDatas { get; set; }
    }
    class BPMData
    {
        public int Bar { get; set; }
        public int BPM { get; set; }
    }

    class Sheet
    {
        public List<Note> Points { get; set; }
    }

    #region Foundations
    public struct Keyframe
    {
        public int Bar { get; set; }
        public int FullBeat { get; set; }
        public int CurBeat { get; set; }
        public int Position { get; set; }
        public int Size { get; set; }
    }
    public abstract class Note : MonoBehaviour
    {
        public Keyframe BeginKeyframe { get; set; }
    }
    public abstract class Unholdable : Note { }
    public abstract class Holdable: Note
    {
        public bool IsDump;
        public Keyframe EndKeyframe { get; set; }
    }
    #endregion

    #region Deployables
    class Tab : Unholdable { }
    class Hold : Holdable { }
    class Slide : Holdable
    {
        public List<Keyframe> Points;
    }
    class AirShort : Unholdable { }
    class AirLong : Holdable { }
    public enum AirMoveDirection { Left, Center, Right }
    class AirMove : Unholdable
    {
        public AirMoveDirection Direction { get; set; }
    }
    #endregion
}