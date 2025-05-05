using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;

namespace UnityLogicGamer.Editor.Util
{
    public static class Utilities
    {
        /// <summary>
        /// 复制文件并加载为指定类型的资源（通常为 TextAsset）
        /// </summary>
        /// <typeparam name="T">UnityEngine.Object 类型，例如 TextAsset</typeparam>
        /// <param name="fileName">文件名数组（不带扩展）</param>
        /// <param name="fileRoot">源文件夹的绝对路径</param>
        /// <param name="targetPath">目标文件夹绝对路径</param>
        /// <param name="nameChange">更换文件名委托</param>
        /// <returns>复制后加载的资源列表</returns>
        public static List<T> CopyAndGetAssets<T>(string[] fileName, string fileRoot, string targetPath, Func<string,string> nameChange = null) where T : UnityEngine.Object
        {
            List<T> result = new List<T>();
            foreach (var name in fileName)
            {
                string sourceFilePath =  Path.Combine(fileRoot,name);
                string targetFullPath =  Path.Combine(targetPath,nameChange != null?nameChange.Invoke(name): name);
                LogicGamer.Core.Utilities.Utility.Directory.CopyFileTo(sourceFilePath,targetFullPath);
                // 转换为 Unity 相对路径（以 "Assets/" 开头）
                string relativePath = GetUnityRelativePath(targetFullPath);
                AssetDatabase.ImportAsset(relativePath);
                var asset = AssetDatabase.LoadAssetAtPath<T>(relativePath);
                if (asset != null)
                {
                    result.Add(asset);
                }
                else
                {
                    Debug.LogWarning($"加载失败: {relativePath}");
                }
            }

            return result;
        }
        /// <summary>
        /// 获取 Unity 项目中的相对路径（以 "Assets/" 开头）
        /// </summary>
        private static string GetUnityRelativePath(string fullPath)
        {
            fullPath = fullPath.Replace("\\", "/");
            string projectPath = Application.dataPath.Replace("Assets", "");
            return fullPath.StartsWith(projectPath)
                ? fullPath.Substring(projectPath.Length)
                : fullPath;
        }
    }
}