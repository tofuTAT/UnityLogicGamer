using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LogicGamer.Core;
using UnityEditor;
using UnityEngine;

namespace UnityLogicGamer.Editor.Build
{
    [Serializable]
    public class QuicklyEntryGeneraterStep:IBuildStep,IDrawEditorView
    {
        [SerializeField] private string _quicklyOutPutRoot = "Assets/QuicklyEntry";
        
        // 需要扫描的程序集
        [SerializeField] private List<string> _assemblies = new List<string>()
        {
            "LogicGamer.Core",
            "UnityLogicGamer.Runtime",
        };
        public void Draw(SerializedProperty property)
        {
            // Draw the _quicklyOutPutRoot field
            SerializedProperty outputRootProp = property.FindPropertyRelative("_quicklyOutPutRoot");
            EditorGUILayout.PropertyField(outputRootProp, new GUIContent("输出路径"));

            // Draw the _assemblies list
            SerializedProperty assembliesProp = property.FindPropertyRelative("_assemblies");
            EditorGUILayout.PropertyField(assembliesProp, new GUIContent("扫描程序集列表"), true);  // 'true' will allow the list to be expandable
        }
        public void Build()
        {
            var targets = _assemblies
                .Select(name => AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                .Where(a => a != null)
                .ToList();
            GeneratorTool.GenerateQuickly(targets,BuildQuickly);
            Debug.Log("Test");
        }
        
        private void BuildQuickly(Dictionary<string, string> fileContents, Dictionary<Type, Assembly> typeAssemblyMap)
        {
            string outputDirectory = _quicklyOutPutRoot;

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            // 清空文件夹，保留 asmdef 和 .meta
            foreach (var file in Directory.GetFiles(outputDirectory))
            {
                if (!file.EndsWith("LogicGamer.QuicklyEntry.asmdef") && !file.EndsWith("LogicGamer.QuicklyEntry.asmdef.meta"))
                {
                    File.Delete(file);
                }
            }

            // 写入 .cs 文件
            foreach (var fileContent in fileContents)
            {
                string filePath = Path.Combine(outputDirectory, $"{fileContent.Key}.cs");
                File.WriteAllText(filePath, fileContent.Value);
                Debug.Log($"生成代码文件: {filePath}");
            }

            // asmdef 路径
            string asmdefPath = Path.Combine(outputDirectory, "LogicGamer.QuicklyEntry.asmdef");

            // 构建新的 asmdef 数据
            var asmdef = new AsmdefData
            {
                name = "LogicGamer.QuicklyEntry",
                references = typeAssemblyMap.Values
                    .Select(a => a.GetName().Name)
                    .Distinct()
                    .ToList(),
                includePlatforms = new List<string>(),
                excludePlatforms = new List<string>(),
                allowUnsafeCode = false,
                overrideReferences = false,
                precompiledReferences = new List<string>(),
                autoReferenced = true,
                defineConstraints = new List<string>(),
                versionDefines = new List<AsmdefData.VersionDefine>(),
                noEngineReferences = false
            };

            // 序列化为 JSON
            string json = JsonUtility.ToJson(asmdef, true);

            // 如果文件不存在，先创建空文件（可选，下面也能自动创建）
            if (!File.Exists(asmdefPath))
            {
                using (File.Create(asmdefPath)) { }
            }

            // 清空原内容并写入新 JSON
            File.WriteAllText(asmdefPath, json);

            Debug.Log($"更新 asmdef 文件: {asmdefPath}");

            AssetDatabase.Refresh();
        }


        
        [Serializable]
        private class AsmdefData
        {
            public string name;
            public string rootNamespace = "";
            public List<string> references = new List<string>();
            public List<string> includePlatforms = new List<string>();
            public List<string> excludePlatforms = new List<string>();
            public bool allowUnsafeCode = false;
            public bool overrideReferences = false;
            public List<string> precompiledReferences = new List<string>();
            public bool autoReferenced = true;
            public List<string> defineConstraints = new List<string>();
            public List<VersionDefine> versionDefines = new List<VersionDefine>();
            public bool noEngineReferences = false;
            
            [Serializable]
            public class VersionDefine
            {
                public string name;
                public string expression;
                public string define;
            }
        }
    }
}