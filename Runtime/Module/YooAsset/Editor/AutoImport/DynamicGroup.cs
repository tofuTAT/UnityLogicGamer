using System.IO;
using YooAsset.Editor;

namespace UnityLogicGamer.Runtime.Module.YooAsset.Editor.AutoImport
{
    public class DynamicGroup : IGroup
    {
        public string GroupName => "Dynamic"; // 组名称

        public void CollectResources(AssetBundleCollectorGroup group)
        {
            string root = "Assets/Res/dynamic"; // 资源的根目录

            // 检查根目录是否存在
            if (Directory.Exists(root))
            {
                // 获取 root 目录下的所有顶级子目录
                string[] subdirectories = Directory.GetDirectories(root);

                // 遍历每个子目录
                foreach (var subdirectory in subdirectories)
                {
                    // 在当前子目录下查找特定的子目录并处理
                    ProcessSubdirectory(subdirectory, group);
                }
            }
        }

        private void ProcessSubdirectory(string subdirectory, AssetBundleCollectorGroup group)
        {
            // 如果该目录存在，则根据不同的目录类型进行处理
            if (Directory.Exists(subdirectory))
            {
                // 获取当前子目录的名称
                string folderName = Path.GetFileName(subdirectory);
                // 根据不同的目录类型调用不同的处理方法
                switch (folderName)
                {
                    case "atlas":
                        ProcessAtlasDirectory(subdirectory, group);
                        break;
                    case "Font":
                        ProcessFontDirectory(subdirectory, group);
                        break;
                    case "music":
                        ProcessMusicDirectory(subdirectory, group);
                        break;
                    case "prefab":
                        ProcessPrefabDirectory(subdirectory, group);
                        break;
                    case "shaderVariants":
                        ProcessShaderVariantsDirectory(subdirectory, group);
                        break;
                    case "spine":
                        ProcessSpineDirectory(subdirectory, group);
                        break;
                    case "texture":
                        ProcessTextureDirectory(subdirectory, group);
                        break;
                    case "config":
                        ProcessConfigDirectory(subdirectory, group);
                        break;
                }
            }
        }

        private void ProcessConfigDirectory(string subdirectory, AssetBundleCollectorGroup group)
        {
            AssetBundleCollector collector = new AssetBundleCollector
            {
                CollectPath = subdirectory, // 设置为文件夹路径
                CollectorType = ECollectorType.DependAssetCollector,
                AddressRuleName = nameof(AddressDisable),
                PackRuleName = nameof(PackTopDirectory),
                FilterRuleName = nameof(CollectAll),
            };
            group.Collectors.Add(collector);  
        }

        private void ProcessAtlasDirectory(string directory, AssetBundleCollectorGroup group)
        {
            AssetBundleCollector collector = new AssetBundleCollector
            {
                CollectPath = directory, // 设置为文件夹路径
                CollectorType = ECollectorType.MainAssetCollector,
                AddressRuleName = nameof(AddressByFileName),
                PackRuleName = nameof(PackSeparately),
                FilterRuleName = nameof(CollectAtlas),
            };
            group.Collectors.Add(collector);  
        }

        private void ProcessFontDirectory(string directory, AssetBundleCollectorGroup group)
        {
            AssetBundleCollector collector = new AssetBundleCollector
            {
                CollectPath = directory, // 设置为文件夹路径
                CollectorType = ECollectorType.MainAssetCollector,
                AddressRuleName = nameof(AddressDisable),
                PackRuleName = nameof(PackDirectory),
                FilterRuleName = nameof(CollectAll),
            };
            group.Collectors.Add(collector);  
        }

        private void ProcessMusicDirectory(string directory, AssetBundleCollectorGroup group)
        {
            AssetBundleCollector collector = new AssetBundleCollector
            {
                CollectPath = directory, // 设置为文件夹路径
                CollectorType = ECollectorType.MainAssetCollector,
                AddressRuleName = nameof(AddressDisable),
                PackRuleName = nameof(PackSeparately),
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
                PackRuleName = nameof(PackSeparately),
                FilterRuleName = nameof(CollectAll),
            };
            group.Collectors.Add(collector);  
        }

        private void ProcessShaderVariantsDirectory(string directory, AssetBundleCollectorGroup group)
        {
            AssetBundleCollector collector = new AssetBundleCollector
            {
                CollectPath = directory, // 设置为文件夹路径
                CollectorType = ECollectorType.MainAssetCollector,
                AddressRuleName = nameof(AddressDisable),
                PackRuleName = nameof(PackDirectory),
                FilterRuleName = nameof(CollectAll),
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
                PackRuleName = nameof(PackTopDirectory),
                FilterRuleName = nameof(CollectAll),
            };
            group.Collectors.Add(collector);  
        }

        private void ProcessTextureDirectory(string directory, AssetBundleCollectorGroup group)
        {
            AssetBundleCollector collector = new AssetBundleCollector
            {
                CollectPath = directory, // 设置为文件夹路径
                CollectorType = ECollectorType.MainAssetCollector,
                AddressRuleName = nameof(AddressDisable),
                PackRuleName = nameof(PackSeparately),
                FilterRuleName = nameof(CollectSprite),
            };
            group.Collectors.Add(collector);  
        }
    }
}