using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInput
{
    bool IsPressed(int key);
    bool IsPressing(int key);
}
