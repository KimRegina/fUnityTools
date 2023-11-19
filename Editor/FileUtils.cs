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
    public struct EditorFile
    {
        public string assetPath;
        public string assetName;
    }

    public class FileUtils
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

        public static EditorFile[] GetTopUnitySubDirectories(string assetPath)
        {
            bool NeedIgnoreFile(string path)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                return !directoryInfo.Exists;
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
            if (!directoryInfo.Exists)
            {
                Debug.LogError($"{assetDirectoryPath} is not Unity Directory");
                return list.ToArray();
            }

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
        public static EditorFile[] GetAllAssetsPathByAssetDirectoryPath(string assetDirectoryPath,
            SearchOption searchOption = SearchOption.AllDirectories,
            Func<string, bool> onCheckIgnore = null)
        {
            FileSystemInfo[] fileSystemInfos =
                GetAllFileSystemInfosByAssetDirectoryPath(assetDirectoryPath, searchOption);
            List<EditorFile> list = new List<EditorFile>();
            for (int i = 0; i < fileSystemInfos.Length; i++)
            {
                FileSystemInfo fileSystemInfo = fileSystemInfos[i];
                if (onCheckIgnore != null && onCheckIgnore(fileSystemInfo.FullName)) continue;
                EditorFile editorFile = new EditorFile();
                editorFile.assetName = fileSystemInfo.Name;
                editorFile.assetPath = fileSystemInfo.FullName.Replace("\\", "/");
                list.Add(editorFile);
            }

            return list.ToArray();
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
            EditorFile[] files =
                GetAllAssetsPathByAssetDirectoryPath(assetDirectoryPath, searchOption, onCheckIgnore);
            UnityEngine.Object[] assets = new UnityEngine.Object[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                assets[i] = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(files[i].assetPath);
            }

            return assets;
        }
    }
}