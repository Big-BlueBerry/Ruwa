using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Note : MonoBehaviour
{
    public float Time { get; set; }
    public int Position { get; set; }
    public int Width { get; set; }
}
