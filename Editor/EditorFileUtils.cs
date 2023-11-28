using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace com.regina.fUnityTools.Editor
{
    /// <summary>
    /// 编辑器下文件信息
    /// </summary>
    public struct EditorAsset
    {
        public string assetPath;
        public string assetName;
    }

    public static class EditorFileUtils
    {
        private static string mApplicationDataPathNoAssets;

        //去掉Application.data的'Assets'
        public static string ApplicationDataPathNoAssets
        {
            get
            {
                if (string.IsNullOrEmpty(mApplicationDataPathNoAssets))
                {
                    mApplicationDataPathNoAssets = Application.dataPath.TrimEnd("Assets".ToCharArray());
                }

                return mApplicationDataPathNoAssets;
            }
        }

        /// <summary>
        /// 判断Asset是否是文件夹
        /// </summary>
        /// <param name="assetPath">Asset路径</param>
        /// <returns>是否Unity文件夹</returns>
        public static bool IsUnityDirectory(string assetPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo($"{ApplicationDataPathNoAssets}/{assetPath}");
            return directoryInfo.Exists;
        }

        /// <summary>
        /// 获取Asset文件夹信息
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static DirectoryInfo GetUnityDirectoryInfo(string assetPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo($"{ApplicationDataPathNoAssets}/{assetPath}");
            return directoryInfo;
        }

        /// <summary>
        /// 获取最上层所有文件夹
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static EditorAsset[] GetTopDirectories(string assetPath)
        {
            bool NeedIgnoreFile(string path)
            {
                FileInfo fileInfo = new FileInfo(path);
                return fileInfo.Exists;
            }

            return GetAllAssetsPathByAssetDirectoryPath(assetPath, SearchOption.TopDirectoryOnly, NeedIgnoreFile);
        }

        /// <summary>
        /// 获取除了 .meta 所有文件(夹)
        /// </summary>
        /// <param name="assetPath">asset路径</param>
        /// <returns></returns>
        public static EditorAsset[] GetTopUnityAssets(string assetPath)
        {
            bool NeedIgnoreFile(string path)
            {
                return path.EndsWith(".meta");
            }

            return GetAllAssetsPathByAssetDirectoryPath(assetPath, SearchOption.TopDirectoryOnly, NeedIgnoreFile);
        }
        
        /// <summary>
        /// 获取Unity文件夹下所有资源，出去忽略文件类型
        /// </summary>
        /// <param name="assetDirectoryPath"></param>
        /// <param name="searchOption"></param>
        /// <param name="pattern"></param>
        /// <param name="onCheckIgnore">检查是否忽略文件Func<'文件path','是否忽略'></param>
        /// <returns></returns>
        public static FileSystemInfo[] GetAllFileSystemInfosByAssetDirectoryPath(string assetDirectoryPath,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo($"{ApplicationDataPathNoAssets}/{assetDirectoryPath}");
            List<FileSystemInfo> list = new List<FileSystemInfo>();
            if (!directoryInfo.Exists) return list.ToArray();

            FileSystemInfo[] files = directoryInfo.GetFileSystemInfos("*", searchOption);
            for (int i = 0; i < files.Length; i++)
            {
                FileSystemInfo fileSystemInfo = files[i];
                if (fileSystemInfo.FullName.EndsWith(".meta")) continue;
                list.Add(fileSystemInfo);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 获取Unity文件夹下所有资源，出去忽略文件类型
        /// </summary>
        /// <param name="assetDirectoryPath"></param>
        /// <param name="searchOption"></param>
        /// <param name="pattern"></param>
        /// <param name="onCheckIgnore">检查是否忽略文件Func<'文件path','是否忽略'></param>
        /// <returns></returns>
        public static EditorAsset[] GetAllAssetsPathByAssetDirectoryPath(string assetDirectoryPath,
            SearchOption searchOption = SearchOption.AllDirectories,
            Func<string, bool> onCheckIgnore = null)
        {
            FileSystemInfo[] fileSystemInfos =
                GetAllFileSystemInfosByAssetDirectoryPath(assetDirectoryPath, searchOption);
            List<EditorAsset> list = new List<EditorAsset>();
            for (int i = 0; i < fileSystemInfos.Length; i++)
            {
                FileSystemInfo fileSystemInfo = fileSystemInfos[i];
                if (onCheckIgnore != null && onCheckIgnore(fileSystemInfo.FullName)) continue;
                EditorAsset editorAsset = ChangeFileSystemInfoToEditorAsset(fileSystemInfo);
                list.Add(editorAsset);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 将FileSystemInfo 转换成 EditorAsset
        /// </summary>
        /// <param name="fileSystemInfo"></param>
        /// <returns></returns>
        private static EditorAsset ChangeFileSystemInfoToEditorAsset(FileSystemInfo fileSystemInfo)
        {
            EditorAsset editorAsset = new EditorAsset();
            editorAsset.assetName = fileSystemInfo.Name;
            string assetPath = fileSystemInfo.FullName.Replace("\\", "/");
            assetPath = assetPath.TrimStart(ApplicationDataPathNoAssets.ToCharArray());
            editorAsset.assetPath = assetPath;
            return editorAsset;
        }
        
        /// <summary>
        /// 获取Unity文件夹下所有文件，出去忽略文件
        /// </summary>
        /// <param name="assetDirectoryPath"></param>
        /// <param name="searchOption"></param>
        /// <param name="pattern"></param>
        /// <param name="onCheckIgnore">检查是否忽略文件Func<'文件path','是否忽略'></param>
        /// <returns></returns>
        public static UnityEngine.Object[] GetAllAssetsByAssetDirectoryPath(string assetDirectoryPath,
            SearchOption searchOption = SearchOption.AllDirectories,
            Func<string, bool> onCheckIgnore = null)
        {
            EditorAsset[] files =
                GetAllAssetsPathByAssetDirectoryPath(assetDirectoryPath, searchOption, onCheckIgnore);
            UnityEngine.Object[] assets = new UnityEngine.Object[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                assets[i] = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(files[i].assetPath);
            }

            return assets;
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