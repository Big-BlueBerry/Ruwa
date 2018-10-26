using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKeyInput : MonoBehaviour, IInput {

    private readonly Dictionary<int, KeyCode> KeyMatch = new Dictionary<int, KeyCode>()
    {
        {1, KeyCode.Q },
        {2, KeyCode.W },
        {3, KeyCode.E },
        {4, KeyCode.R },
        {5, KeyCode.T },
        {6, KeyCode.Y },
        {7, KeyCode.U },
        {8, KeyCode.I },
        {9, KeyCode.O },
        {10, KeyCode.P },
        {11, KeyCode.LeftBracket },
        {12, KeyCode.RightBracket },
        {13, KeyCode.Backslash },
        {14, KeyCode.Keypad7 },
        {15, KeyCode.Keypad8 },
        {16, KeyCode.Keypad9 },
    };

    public bool IsPressed(int key)
    {
        return Input.GetKeyDown(KeyMatch[key]);
    }

    public bool IsPressing(int key)
    {
        return Input.GetKey(KeyMatch[key]);
    }
}
