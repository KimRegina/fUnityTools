using System;
using UnityEditor;
using UnityEngine;

namespace com.regina.fUnityTools.Editor
{
    [InitializeOnLoad]
    public static class EditorApplicationExtension
    {
        private static event Action editorUpdateEvent;

        static EditorApplicationExtension()
        {
            EditorApplication.update = OnEditorUpdate;
        }

        public static void Subscribe(Action action)
        {
            editorUpdateEvent += action;
        }

        public static void UnSubscribe(Action action)
        {
            editorUpdateEvent -= action;
        }

        private static void OnEditorUpdate()
        {
            editorUpdateEvent?.Invoke();
        }
    }
}