using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace com.regina.fUnityTools.Editor
{
    public static class EditorFileUtils
    {
        private static string mApplicationDataPathNoAssets;

        //去掉Application.data的'/Assets'
        public static string ApplicationDataPathNoAssets
        {
            get
            {
                if (string.IsNullOrEmpty(mApplicationDataPathNoAssets))
                {
                    mApplicationDataPathNoAssets =
                        Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length);
                }

                return mApplicationDataPathNoAssets;
            }
        }

        /// <summary>
        /// 判断Asset是否是文件夹
        /// </summary>
        /// <param name="assetPath">Asset路径</param>
        /// <returns>是否Unity文件夹</returns>
        public static bool IsDirectory(string assetPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo($"{ApplicationDataPathNoAssets}/{assetPath}");
            return directoryInfo.Exists;
        }

        /// <summary>
        /// 获取Asset文件夹信息
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static DirectoryInfo GetDirectoryInfo(string assetPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo($"{ApplicationDataPathNoAssets}/{assetPath}");
            return directoryInfo;
        }

        /// <summary>
        /// 获取最上层所有文件夹
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string[] GetTopDirectoryPaths(string assetPath)
        {
            bool NeedIgnoreFile(string path)
            {
                FileInfo fileInfo = new FileInfo(path);
                return fileInfo.Exists;
            }

            return GetTopAssetPaths(assetPath, NeedIgnoreFile);
        }

        /// <summary>
        /// 获取Unity文件夹下所有资源，出去忽略文件类型
        /// </summary>
        /// <param name="assetDirectoryPath"></param>
        /// <param name="searchOption"></param>
        /// <param name="pattern"></param>
        /// <param name="onCheckIgnore">检查是否忽略文件Func<'文件path','是否忽略'></param>
        /// <returns></returns>
        public static string[] GetTopAssetPaths(string directoryPath,
            Func<string, bool> onCheckIgnore, string filter = "*")
        {
            List<string> list = new List<string>();
            DirectoryInfo directoryInfo = new DirectoryInfo($"{ApplicationDataPathNoAssets}/{directoryPath}");
            if (!directoryInfo.Exists) return list.ToArray();
            FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos(filter, SearchOption.TopDirectoryOnly);
            for (int i = 0; i < fileSystemInfos.Length; i++)
            {
                FileSystemInfo fileSystemInfo = fileSystemInfos[i];
                string fullPath = fileSystemInfo.FullName.Replace('\\', '/');
                if (onCheckIgnore != null && onCheckIgnore(fullPath)) continue;
                string assetPath = fullPath.Substring(ApplicationDataPathNoAssets.Length + 1);
                list.Add(assetPath);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 获取除了 .meta 所有文件(夹)
        /// </summary>
        /// <param name="assetPath">asset路径</param>
        /// <returns></returns>
        public static string[] GetTopAssetPaths(string assetPath)
        {
            bool NeedIgnoreFile(string path)
            {
                return path.EndsWith(".meta");
            }

            return GetTopAssetPaths(assetPath, NeedIgnoreFile);
        }

        /// <summary>
        /// 获取{directoriesPath}下所有Asset的路径
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="directoriesPath"></param>
        /// <returns></returns>
        public static string[] FindAllAssetsPath(string filter, params string[] directoriesPath)
        {
            return AssetDatabase.FindAssets(filter, directoriesPath);
        }

        /// <summary>
        /// 获取{directoriesPath}下所有Asset
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="directoriesPath"></param>
        /// <typeparam name="T">UnityEngine.Object</typeparam>
        /// <returns></returns>
        public static T[] FindAllAssets<T>(string filter, params string[] directoriesPath) where T : UnityEngine.Object
        {
            string[] guids = FindAllAssetsPath(filter, directoriesPath);
            List<T> assetList = new List<T>();
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T target = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (target == null) continue;
                assetList.Add(target);
            }

            return assetList.ToArray();
        }

        public static void WriteAssetFile(string content, string assetPath)
        {
            string filePath = $"{ApplicationDataPathNoAssets}/{assetPath}";
            File.WriteAllText(filePath, content);
            AssetDatabase.Refresh();
        }

        public static void DeleteAssetFile(string assetPath)
        {
            string filePath = $"{ApplicationDataPathNoAssets}/{assetPath}";
            FileInfo file = new FileInfo(filePath);
            if (file.Exists) file.Delete();
            else Debug.LogError($"{filePath} not existed");
            AssetDatabase.Refresh();
        }
    }
}