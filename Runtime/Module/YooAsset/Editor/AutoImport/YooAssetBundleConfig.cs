using System;
using System.Linq;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

namespace UnityLogicGamer.Runtime.Module.YooAsset.Editor.AutoImport
{
    public enum VersionType
    {
        AutoByTime,
        Input,
    }

    public abstract class YooAssetBundleConfig: ScriptableObject
    {
        protected abstract EBuildPipeline BuildPipeline { get; }
        public abstract void ExecuteBuild();
        
        [SerializeField]protected string packageName;  // AssetBundle 名称
        [SerializeField]protected string shaderVariantsPath;  // AssetBundle 名称

        public string ShaderVariantsPath
        {
            get
            {
                if (string.IsNullOrEmpty(shaderVariantsPath))
                {
                   return $"Assets/{packageName}ShaderVariants.shadervariants";
                }
                return shaderVariantsPath;
            }
        }
        public string PackageName => packageName;


        [SerializeField]
        protected VersionType versionType = VersionType.AutoByTime;

        [SerializeField] protected string inputVersion;


        [SerializeField] 
        private string encryption = typeof(EncryptionNone).FullName;

        [SerializeField]
        protected EFileNameStyle fileNameStyle;

        [SerializeField]protected EBuildinFileCopyOption copyBuildinFileOptionField;
        protected bool ShowInputVersion()
        {
           return versionType == VersionType.Input;
        }
        
        
        protected IEncryptionServices CreateEncryptionInstance()
        {
            var encyptionClassName = AssetBundleBuilderSetting.GetPackageEncyptionClassName(packageName, BuildPipeline);
            var encryptionClassTypes = EditorTools.GetAssignableTypes(typeof(IEncryptionServices));
            var classType = encryptionClassTypes.Find(x => x.FullName.Equals(encyptionClassName));
            if (classType != null)
                return (IEncryptionServices)Activator.CreateInstance(classType);
            else
                return null;
        }
        protected virtual string GetVersion()
        {
            switch (versionType)
            {
                case VersionType.AutoByTime:
                    int totalMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                    return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
                case VersionType.Input:
                    if (string.IsNullOrEmpty(inputVersion))
                    {
                        throw new InvalidOperationException("Input version cannot be empty.");
                    }
                    return inputVersion;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
        
        // 使用 Odin 序列化支持
        [SerializeField] private string[] groups;

        public IGroup[] GetGroups()
        {
            IGroup[] temp = null;
            if (groups != null)
            {
                temp = new IGroup[groups.Length];
                for (int i = 0; i < groups.Length; i++)
                {
                    var item = groups[i];
                    var type = Type.GetType(item);
                    temp[i] = (IGroup)Activator.CreateInstance(type);
                }
            }

            return temp;
        }

        public void AutoImport()
        {
            // 检查是否有 PackageName 这个包裹
            var package = AssetBundleCollectorSettingData.Setting.Packages
                .FirstOrDefault(p => p.PackageName == packageName);

            // 清空整个包裹（如果存在的话），否则直接创建新包裹
            if (package != null)
            {
                AssetBundleCollectorSettingData.RemovePackage(package); // 删除包裹
            }
            // 重新创建包裹
            package = AssetBundleCollectorSettingData.CreatePackage(packageName);
            package.EnableAddressable = true;
            var tempGroups = GetGroups();
            //获取程序集内所有Group
            foreach (var groupInstance in tempGroups)
            {
                //检查是否有重名 重名则报错
                // 检查该 Group 是否已存在同名的分组
                if (package.Groups.Any(g => g.GroupName == groupInstance.GroupName))
                {
                    throw new Exception($"分组名称冲突：'{groupInstance.GroupName}' 已经存在于包裹 '{package.PackageName}' 中！");  // 如果有重名的分组，跳过当前分组，不进行处理
                }
                AssetBundleCollectorGroup group = AssetBundleCollectorSettingData.CreateGroup(package, groupInstance.GroupName);
                //各组自己处理资源
                groupInstance.CollectResources(group);
            }
            // 保存配置文件
            AssetBundleCollectorSettingData.SaveFile();
        }
        
    }
}