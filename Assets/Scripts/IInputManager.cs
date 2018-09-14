using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputManager
{
    bool IsKeyDown(int key);
    bool IsKeyPressed(int key);
    bool IsKeyReleased(int key);
}
