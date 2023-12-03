using System;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;
using static System.Reflection.BindingFlags;

namespace com.regina.fUnityTools.Editor
{
    public class EditorExtension
    {
        public static string[] GetAllLabels()
        {
            var tmpDic = typeof(AssetDatabase)
                .GetMethod("GetAllLabels", Static | NonPublic)
                ?.Invoke(null, null) as Dictionary<string, float>;

            if (tmpDic == null) return Array.Empty<string>();
            return tmpDic.Keys.ToArray();
        }
    }
}