using UnityEditor;

//********************************************************
// % (ctrl on Windows and Linux, cmd on macOS),
// ^ (ctrl on Windows, Linux, and macOS),
// # (shift),
// & (alt).
////********************************************************

public class EditorMenu : Editor
{
    [MenuItem("Tools/ShortCuts/开关gameObjects显示 %&A")]
    public static void SetSelectionsActive()
    {
        var list = Selection.gameObjects;
        if (list == null || list.Length == 0) return;
        bool isActive = list[0].activeInHierarchy;
        for (int i = 0; i < list.Length; i++)
        {
            list[i].SetActive(!isActive);
        }
    }
    
}