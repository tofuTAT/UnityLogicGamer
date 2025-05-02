using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LogicGamer.Core;
using UnityEditor;
using UnityEngine;
using UnityLogicGamer.Editor.Tool;

namespace UnityLogicGamer.Editor.Config
{
    [FilePath("ProjectSettings/UnityLogicGamerEditorConfig.asset", FilePathAttribute.Location.ProjectFolder)]
    public class UnityLogicGamerEditorConfig : ScriptableSingleton<UnityLogicGamerEditorConfig>
    {
        // 可自定义的路径
        [SerializeField]
        private string _quicklyOutPutRoot = "Assets/QuicklyEntry";

        public string QuicklyOutPutRoot => _quicklyOutPutRoot;

        private static List<string> _defaultAssemblies = new()
        {
            "LogicGamer.Core",
            "UnityLogicGamer.Runtime",
        };

        // 需要额外扫描的程序集
        [SerializeField]
        private List<string> _assemblies = new List<string>();

        public IEnumerable<string> Assemblies => _assemblies.Concat(_defaultAssemblies);


        // 提供一个 Project Settings 面板
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            var provider = new SettingsProvider("Project/LogicGamer", SettingsScope.Project)
            {
                label = "Unity Logic Gamer",
                guiHandler = searchContext =>
                {
                    var config = instance;
                    SerializedObject so = new SerializedObject(config);
                    EditorGUI.BeginChangeCheck();

                    // 显示 QuicklyEntry 输出路径部分，作为最重要的配置区域
                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField("QuicklyEntry 配置", EditorStyles.boldLabel);
                    EditorGUILayout.Space(5);

                    // QuicklyEntry 输出路径字段
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("输出路径", GUILayout.Width(150));
                    EditorGUILayout.SelectableLabel(config._quicklyOutPutRoot, EditorStyles.textField, GUILayout.Height(18));

                    if (GUILayout.Button("浏览", GUILayout.Width(60)))
                    {
                        string path = EditorUtility.OpenFolderPanel("选择输出路径", "Assets", "");
                        if (!string.IsNullOrEmpty(path))
                        {
                            if (path.StartsWith(Application.dataPath))
                            {
                                config._quicklyOutPutRoot = "Assets" + path.Substring(Application.dataPath.Length);
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("路径错误", "请选择 Assets 文件夹内的路径。", "确定");
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
           
                    // 显示默认扫描程序集，作为不可修改区域
                    EditorGUILayout.LabelField("默认扫描程序集（不可修改）");
                    foreach (var asm in _defaultAssemblies)
                    {
                        EditorGUILayout.LabelField("- " + asm, EditorStyles.miniLabel);
                    }
                    // 用户自定义程序集列表
                    EditorGUILayout.LabelField("额外扫描的程序集列表");
                    SerializedProperty assembliesProperty = so.FindProperty("_assemblies");
                    EditorGUILayout.PropertyField(assembliesProperty, true);
                    // 增加“生成”按钮
                    if (GUILayout.Button("生成", GUILayout.Height(30)))
                    {
                        List<Assembly> assemblies = new List<Assembly>();
                        // 尝试加载每个程序集
                        foreach (var item in config.Assemblies)
                        {
                            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                                .FirstOrDefault(a => a.GetName().Name.Equals(item, StringComparison.OrdinalIgnoreCase));
                            if (assembly != null)
                            {
                                assemblies.Add(assembly);
                            }
                            else
                            {
                                Debug.LogWarning($"程序集 '{item}' 未找到");
                            }
                        }
                        GeneratorTool.GenerateQuickly(assemblies,Generater.BuildQuickly);
                    }
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    if (EditorGUI.EndChangeCheck())
                    {
                        so.ApplyModifiedProperties();
                        config.Save(true);
                    }
                }
            };
            return provider;
        }
    }
}
