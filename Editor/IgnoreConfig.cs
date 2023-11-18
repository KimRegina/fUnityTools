using UnityEngine;

namespace com.regina.fUnityTools.Editor
{
    [CreateAssetMenu(fileName = "IgnoreConfig",menuName = "Config/CreateIgnoreCofig")]
    public class IgnoreConfig : ScriptableObject
    {
        [Tooltip("忽略的文件后缀名")]
        public string[] extensions;
    }
}