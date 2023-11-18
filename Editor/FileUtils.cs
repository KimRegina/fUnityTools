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

        private static IgnoreConfig mIgnoreConfig;

        /// <summary>
        /// 获取文件忽略配置
        /// </summary>
        public static IgnoreConfig ignoreConfig
        {
            get
            {
                if (mIgnoreConfig == null)
                    mIgnoreConfig =
                        AssetDatabase.LoadAssetAtPath<IgnoreConfig>(
                            "Assets/com.regina.fUnityTools/Editor/Config/IgnoreConfig.asset");
                return mIgnoreConfig;
            }
        }

        private static string[] mIgnoreExtensions;

        /// <summary>
        /// 忽略文件后缀列表
        /// </summary>
        public static string[] ignoreExtensions
        {
            get
            {
                if (mIgnoreExtensions == null)
                {
                    if (ignoreConfig != null) mIgnoreExtensions = ignoreConfig.extensions;
                    else mIgnoreExtensions = Array.Empty<string>();
                }

                return mIgnoreExtensions;
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

        public static string[] GetAllUnitySubDirectories(string assetPath)
        {
            return null;
        }

        /// <summary>
        /// 获取Unity文件夹下所有资源，出去忽略文件类型
        /// </summary>
        /// <param name="assetDirectoryPath"></param>
        /// <param name="searchOption"></param>
        /// <param name="pattern"></param>
        /// <param name="ignoreGlobalConfig">是否启用全局忽略配置</param>
        /// <returns></returns>
        public static string[] GetAllAssetsPathByAssetDirectoryPath(string assetDirectoryPath,
            SearchOption searchOption = SearchOption.AllDirectories, string pattern = "*.*", bool ignoreGlobalConfig = true)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo($"{ApplicationDataPathNoAssets}/{assetDirectoryPath}");
            List<string> list = new List<string>();
            if (!directoryInfo.Exists)
            {
                Debug.LogError($"{assetDirectoryPath} is not Unity Directory");
                return list.ToArray();
            }

            FileSystemInfo[] files = directoryInfo.GetFileSystemInfos(pattern, searchOption);
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i].FullName.Replace("\\", "/");
                if(ignoreGlobalConfig && IsIgnoreFile(filePath)) continue;
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
        /// <param name="ignoreGlobalConfig"></param>
        /// <returns></returns>
        public static UnityEngine.Object[] GetAllAssetsByAssetDirectoryPath(string assetDirectoryPath,
            SearchOption searchOption = SearchOption.AllDirectories,string pattern = "*.*",bool ignoreGlobalConfig = true)
        {
            string[] files = GetAllAssetsPathByAssetDirectoryPath(assetDirectoryPath, searchOption,pattern,ignoreGlobalConfig);
            UnityEngine.Object[] assets = new UnityEngine.Object[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                assets[i] = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(files[i]);
            }

            return assets;
        }
        
        public static bool IsIgnoreFile(string filePath)
        {
            for (int i = 0; i < ignoreExtensions.Length; i++)
            {
                if (filePath.EndsWith(ignoreExtensions[i]))
                    return true;
            }

            return false;
        }
    }
}