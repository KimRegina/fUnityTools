using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace com.regina.fUnityTools.Editor
{
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

        public static string[] GetTopUnitySubDirectories(string assetPath)
        {
            bool IsDirectory(string path)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                return directoryInfo.Exists;
            }

            return GetAllAssetsPathByAssetDirectoryPath(assetPath, SearchOption.TopDirectoryOnly, IsDirectory);
        }

        /// <summary>
        /// 获取Unity文件夹下所有资源，出去忽略文件类型
        /// </summary>
        /// <param name="assetDirectoryPath"></param>
        /// <param name="searchOption"></param>
        /// <param name="pattern"></param>
        /// <param name="onCheckIgnore">检查是否忽略文件Func<'文件path','是否忽略'></param>
        /// <returns></returns>
        public static string[] GetAllAssetsPathByAssetDirectoryPath(string assetDirectoryPath,
            SearchOption searchOption = SearchOption.AllDirectories,
            Func<string, bool> onCheckIgnore = null)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo($"{ApplicationDataPathNoAssets}/{assetDirectoryPath}");
            List<string> list = new List<string>();
            if (!directoryInfo.Exists)
            {
                Debug.LogError($"{assetDirectoryPath} is not Unity Directory");
                return list.ToArray();
            }

            FileSystemInfo[] files = directoryInfo.GetFileSystemInfos("*", searchOption);
            for (int i = 0; i < files.Length; i++)
            {
                FileSystemInfo fileSystemInfo = files[i];
                string filePath = fileSystemInfo.FullName.Replace("\\", "/");
                if (onCheckIgnore != null && onCheckIgnore(filePath)) continue;
                string assetPath = filePath.TrimStart(ApplicationDataPathNoAssets.ToCharArray());
                list.Add(assetPath);
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
            string[] files =
                GetAllAssetsPathByAssetDirectoryPath(assetDirectoryPath, searchOption, onCheckIgnore);
            UnityEngine.Object[] assets = new UnityEngine.Object[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                assets[i] = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(files[i]);
            }

            return assets;
        }
    }
}