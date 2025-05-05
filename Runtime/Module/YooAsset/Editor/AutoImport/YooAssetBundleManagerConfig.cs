using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace UnityLogicGamer.Runtime.Module.YooAsset.Editor.AutoImport
{
    public class YooAssetBundleManagerConfig : ScriptableObject
    {

        // 配置文件存放的根目录
        public const string YooAssetBundleConfigRoot = "Assets/YooAssetBundleConfigs";

        private static YooAssetBundleManagerConfig instance;

        // 单例实例的访问属性
        public static YooAssetBundleManagerConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    // 尝试加载配置文件
                    string assetPath = $"{YooAssetBundleConfigRoot}/YooAssetBundleManagerConfig.asset";
                    instance = AssetDatabase.LoadAssetAtPath<YooAssetBundleManagerConfig>(assetPath);
                    // 如果配置文件不存在，创建新的实例
                    if (instance == null)
                    {
                        instance = CreateInstance<YooAssetBundleManagerConfig>();

                        // 确保配置文件目录存在
                        if (!System.IO.Directory.Exists(YooAssetBundleConfigRoot))
                        {
                            System.IO.Directory.CreateDirectory(YooAssetBundleConfigRoot);
                        }

                        // 保存新的实例到磁盘
                        AssetDatabase.CreateAsset(instance, assetPath);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }

                return instance;
            }
        }


        // 所有子类类型
        private Type[] availableConfigTypes;

        // 点击按钮后，直接显示下拉框选择
        [Button("Create New AssetBundle Config")]
        private void ShowCreateConfigMenu()
        {
            // 每次点击按钮时动态获取子类类型
            availableConfigTypes = GetAvailableConfigTypes();

            if (availableConfigTypes == null || availableConfigTypes.Length == 0)
            {
                Debug.LogError("没有找到可创建的子类！");
                return;
            }

            // 显示下拉框
            ShowPopupAtMousePosition();
        }

        // 显示下拉框并根据鼠标位置显示
        private void ShowPopupAtMousePosition()
        {
            // 获取鼠标位置
            Vector2 mousePosition = Event.current.mousePosition;

            // 创建并显示 GenericMenu
            GenericMenu menu = new GenericMenu();

            // 为每个可用的类型添加菜单项
            for (int i = 0; i < availableConfigTypes.Length; i++)
            {
                Type configType = availableConfigTypes[i];
                menu.AddItem(new GUIContent(configType.Name), false, () => CreateAndSaveConfig(configType));
            }

            // 显示菜单在鼠标当前位置
            menu.DropDown(new Rect(mousePosition.x, mousePosition.y, 0, 0));
        }

        // 创建并保存选择的子类实例
        private void CreateAndSaveConfig(Type configType)
        {
            // 使用反射创建所选类型的实例
            var newConfig = ScriptableObject.CreateInstance(configType);

            // 保存新实例到磁盘
            string assetPath = $"{YooAssetBundleConfigRoot}/{configType.Name}.asset";
            AssetDatabase.CreateAsset(newConfig, assetPath);
            AssetDatabase.SaveAssets();

            // 聚焦并选中新创建的资产
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newConfig;
        }

        // 获取所有实现了 YooAssetBundleConfig 的子类
        private Type[] GetAvailableConfigTypes()
        {
            // 动态获取所有实现了 YooAssetBundleConfig 的子类类型
            var baseType = typeof(YooAssetBundleConfig);
            var allTypes = Assembly.GetAssembly(baseType)
                .GetTypes()
                .Where(t => baseType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToArray();
            return allTypes;
        }

        [SerializeField] private List<YooAssetBundleConfig> autoBundle;

        [ValueDropdown("GetBuildHandles")] [SerializeField]
        private string[] autoBuildHandle;


        public void Build()
        {
            if (autoBundle == null || autoBundle.Count == 0)
            {
                Debug.LogWarning("请配置需要打包的设置");
                return;
            }
            foreach (var item in Instance.autoBundle)
            {
                item.AutoImport();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                item.ExecuteBuild();
            }
        }
        
        public void AutoCollect()
        {
            if (autoBundle == null || autoBundle.Count == 0)
            {
                Debug.LogWarning("请配置需要打包的设置");
                return;
            }

            foreach (var item in Instance.autoBundle)
            {
                item.AutoImport();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

    }
}