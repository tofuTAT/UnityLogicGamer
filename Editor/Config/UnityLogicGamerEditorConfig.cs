using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LogicGamer.Core.Utilities;
using UnityEditor;
using UnityEngine;
using UnityLogicGamer.Editor.Build;

namespace UnityLogicGamer.Editor.Config
{
    [FilePath("ProjectSettings/UnityLogicGamerEditorConfig.asset", FilePathAttribute.Location.ProjectFolder)]
    public class UnityLogicGamerEditorConfig : ScriptableSingleton<UnityLogicGamerEditorConfig>
    {
        public static string ProjectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        
        [SerializeReference] private List<IBuildStep> builds;
        // 缓存折叠状态的字典
        private static Dictionary<int, bool> foldouts = new Dictionary<int, bool>();
        // 提供一个 Project Settings 面板
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            var provider = new SettingsProvider("Project/UnityLogicGamer", SettingsScope.Project)
            {
                label = "Unity Logic Gamer",
                guiHandler = searchContext =>
                {
                    var config = instance;
                    SerializedObject so = new SerializedObject(config);
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.Space(10);

                    #region BuildStep
                    EditorGUILayout.BeginHorizontal();
                    // 显示 Build Steps 列表
                    EditorGUILayout.LabelField("Build Steps 配置 (按顺序执行)", EditorStyles.boldLabel);
                    // 添加 Build Step 按钮
                    if (GUILayout.Button("新增Step",GUILayout.Width(100)))
                    {
                        // 弹出类型选择框
                        var availableTypes = Utility.Type.GetTypesImplementing<IBuildStep>()
                            .Where(t => t.GetCustomAttribute<SerializableAttribute>() != null)
                            .ToList();

                        // 弹出菜单并选择要添加的类型
                        GenericMenu menu = new GenericMenu();
                        foreach (var type in availableTypes)
                        {
                            string name = type.Name;
                            menu.AddItem(new GUIContent(name), false, () =>
                            {
                                // 在选择后实例化并添加到 list 中
                                config.builds.Add((IBuildStep)Activator.CreateInstance(type));
                            });
                        }

                        // 如果没有可选的 BuildStep 类型，添加一个禁用项
                        if (!availableTypes.Any())
                        {
                            menu.AddDisabledItem(new GUIContent("未找到任何可添加的 Build Step 类型"));
                        }

                        // 显示菜单
                        menu.ShowAsContext();
                    }

                    if (GUILayout.Button("全部执行", GUILayout.Width(100)))
                    {
                        config.BuildAll();
                        return;
                    }

                    EditorGUILayout.EndHorizontal();
                    SerializedProperty buildsProp = so.FindProperty("builds");
                    // 手动绘制每个 Build Step
                    for (int i = 0; i < buildsProp.arraySize; i++)
                    {
                        SerializedProperty element = buildsProp.GetArrayElementAtIndex(i);
                        object instance = element.managedReferenceValue;
                        // 检查并获取每个步骤的折叠状态
                        if (!foldouts.ContainsKey(i))
                        {
                            foldouts[i] = false; // 默认展开
                        }
                        // 使用 BeginHorizontal() 开始水平布局
                        EditorGUILayout.BeginHorizontal();
                        // 使用折叠标题显示 Build Step
                        foldouts[i] = EditorGUILayout.Foldout(foldouts[i], $"步骤 {i + 1} - {instance?.GetType().Name}", true);
                        // 上移按钮
                        if (GUILayout.Button("上移", GUILayout.Width(60)) && i > 0)
                        {
                            buildsProp.MoveArrayElement(i, i - 1);
                        }

                        // 下移按钮
                        if (GUILayout.Button("下移", GUILayout.Width(60)) && i < buildsProp.arraySize - 1)
                        {
                            buildsProp.MoveArrayElement(i, i + 1);
                        }

                        // 执行按钮
                        if (GUILayout.Button("执行", GUILayout.Width(60)) && instance is IBuildStep step)
                        {
                            step.Build();
                        }

                        // 删除按钮
                        if (GUILayout.Button("删除", GUILayout.Width(60)))
                        {
                            buildsProp.DeleteArrayElementAtIndex(i);
                            break;  // 防止越界
                        }
                        EditorGUILayout.EndHorizontal();
                        
                        if (foldouts[i])
                        {
                        
                            EditorGUILayout.BeginVertical("box");
                            // 如果该元素实现了 IDrawEditorView 接口，则调用其 Draw 方法
                            if (instance is IDrawEditorView drawer)
                            {
                                drawer.Draw(element);  // 调用子类的 Draw 方法，传递 SerializedProperty
                            }
                            else
                            {
                                // 默认序列化显示
                                EditorGUILayout.PropertyField(element, new GUIContent($"Build Step {i + 1}"), true);
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    EditorGUILayout.Space();
                    

                    #endregion
                   


                    if (EditorGUI.EndChangeCheck())
                    {
                        so.ApplyModifiedProperties();
                        config.Save(true);
                    }
                }
            };
            return provider;
        }
        public void BuildAll()
        {
            Debug.Log("打包开始");
            for (int i = 0; i < builds.Count; i++)
            {
                var build =builds[i];
                Debug.Log($"build Step {i+1} {build.GetType()} 开始");
                build.Build();
                Debug.Log($"build Step {i+1} {build.GetType()} 完成");
            }
        }
        [MenuItem("UnityLogicGamer/Settings")]
        public static void Open()
        {
            SettingsService.OpenProjectSettings("Project/UnityLogicGamer");
        }
        
        [MenuItem("UnityLogicGamer/Build/All")]
        public static void ExecuteAllSteps()
        {
            instance.BuildAll();
        }
    }
}