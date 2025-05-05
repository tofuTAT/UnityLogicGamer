using System.IO;
using YooAsset.Editor;

namespace UnityLogicGamer.Runtime.Module.YooAsset.Editor.AutoImport
{
    public class StaticGroup : IGroup
    {
        public string GroupName => "Static";  // 组名称

        public void CollectResources(AssetBundleCollectorGroup group)
        {
            string root = "Assets/Res/static";  // 资源的根目录

            // 检查根目录是否存在
            if (Directory.Exists(root))
            {
                // 获取所有顶级子目录
                string[] subdirectories = Directory.GetDirectories(root);

                // 遍历每个子目录
                foreach (var subdirectory in subdirectories)
                {
                    // 在每个子目录下查找特定的子目录并处理
                    ProcessSubdirectory(subdirectory, group);
                }
            }
        }

        private void ProcessSubdirectory(string subdirectory, AssetBundleCollectorGroup group)
        {
            // 定义需要查找的目录名
            string[] targetDirs = { "sprite","atlas", "mat", "prefab", "spine", "shader","config" };

            foreach (var targetDir in targetDirs)
            {
                // 获取当前子目录下指定的子目录路径
                string targetPath = Path.Combine(subdirectory, targetDir);

                // 如果该目录存在，则根据不同的目录类型进行处理
                if (Directory.Exists(targetPath))
                {
                    // 根据不同的目录类型调用不同的处理方法
                    switch (targetDir)
                    {
                        case "sprite":
                            ProcessSpriteDirectory(targetPath, group);
                            break;
                        case "mat":
                            ProcessMatDirectory(targetPath, group);
                            break;
                        case "prefab":
                            ProcessPrefabDirectory(targetPath, group);
                            break;
                        case "spine":
                            ProcessSpineDirectory(targetPath, group);
                            break;
                        case "shader":
                            ProcessShaderDirectory(targetPath, group);
                            break;
                        case "config":
                            ProcessConfigDirectory(targetPath, group);
                            break;
                    }
                }
            }
        }

        private void ProcessConfigDirectory(string targetPath, AssetBundleCollectorGroup group)
        {
            // 假设在 atlas 目录下收集 .atlas 文件
            AssetBundleCollector collector = new AssetBundleCollector
            {
                CollectPath = targetPath, // 设置为文件夹路径
                CollectorType = ECollectorType.DependAssetCollector,
                AddressRuleName = nameof(AddressDisable),
                PackRuleName = nameof(PackDirectory),
                FilterRuleName = nameof(CollectAll),
            };
            group.Collectors.Add(collector);  
        }

        private void ProcessSpriteDirectory(string directory, AssetBundleCollectorGroup group)
        {
            // 假设在 atlas 目录下收集 .atlas 文件
            AssetBundleCollector collector = new AssetBundleCollector
            {
                CollectPath = directory, // 设置为文件夹路径
                CollectorType = ECollectorType.StaticAssetCollector,
                AddressRuleName = nameof(AddressByFileName),
                PackRuleName = nameof(PackDirectory),
                FilterRuleName = nameof(CollectAll),
            };
            group.Collectors.Add(collector);  
        }

        private void ProcessMatDirectory(string directory, AssetBundleCollectorGroup group)
        {
            AssetBundleCollector collector = new AssetBundleCollector
            {
                CollectPath = directory, // 设置为文件夹路径
                CollectorType = ECollectorType.DependAssetCollector,
                AddressRuleName = nameof(AddressDisable),
                PackRuleName = nameof(PackDirectory),
                FilterRuleName = nameof(CollectAll),
            };
            group.Collectors.Add(collector);  
        }

        private void ProcessPrefabDirectory(string directory, AssetBundleCollectorGroup group)
        {
            AssetBundleCollector collector = new AssetBundleCollector
            {
                CollectPath = directory, // 设置为文件夹路径
                CollectorType = ECollectorType.MainAssetCollector,
                AddressRuleName = nameof(AddressDisable),
                PackRuleName = nameof(PackDirectory),
                FilterRuleName = nameof(CollectPrefab),
            };
            group.Collectors.Add(collector);  
        }

        private void ProcessSpineDirectory(string directory, AssetBundleCollectorGroup group)
        {
            AssetBundleCollector collector = new AssetBundleCollector
            {
                CollectPath = directory, // 设置为文件夹路径
                CollectorType = ECollectorType.DependAssetCollector,
                AddressRuleName = nameof(AddressDisable),
                PackRuleName = nameof(PackDirectory),
                FilterRuleName = nameof(CollectAll),
            };
            group.Collectors.Add(collector); 
        }

        private void ProcessShaderDirectory(string directory, AssetBundleCollectorGroup group)
        {
            AssetBundleCollector collector = new AssetBundleCollector
            {
                CollectPath = directory, // 设置为文件夹路径
                CollectorType = ECollectorType.DependAssetCollector,
                AddressRuleName = nameof(AddressDisable),
                PackRuleName = nameof(PackDirectory),
                FilterRuleName = nameof(CollectShader),
            };
            group.Collectors.Add(collector); 
        }
    }
}
