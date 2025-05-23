using UnityEngine;

public static class CursorUtils
{
    public static void SetCursor(CursorInfo info)
    {
        Debug.Log($"[Cursor] Setting cursor settings! {info.lockMode} | {info.hidden}");
        Cursor.lockState = info.lockMode;
        Cursor.visible = !info.hidden;
    }
}

[System.Serializable]
public struct CursorInfo
{
    public CursorLockMode lockMode;
    public bool hidden;
}
