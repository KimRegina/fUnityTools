using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorTest 
{

    [MenuItem("Test/TestFunc1")]
    public static void TestFunc1()
    {
        var list = com.regina.fUnityTools.Editor.EditorFileUtils.GetAllAssetsByAssetDirectoryPath(
            "Assets/com.regina.fUnityTools/Editor/Config");

        for (int i = 0; i < list.Length; i++)
        {
            Debug.Log(list[i]);
        }
    }

}
