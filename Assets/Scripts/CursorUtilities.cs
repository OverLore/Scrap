using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CursorUtilities
{
    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public enum ClickType
    {
        Left,
        Right,
        Middle,
        ShiftLeft,
        ShiftRight,
        None
    }
}
