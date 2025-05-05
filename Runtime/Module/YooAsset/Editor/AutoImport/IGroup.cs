using YooAsset.Editor;

namespace UnityLogicGamer.Runtime.Module.YooAsset.Editor.AutoImport
{
    public interface IGroup
    {
        string GroupName { get; }

        /// <summary>
        /// 收集该组的资源，并根据 AssetBundleCollectorGroup 进行管理
        /// </summary>
        /// <param name="group">资源分组</param>
        void CollectResources(AssetBundleCollectorGroup group);
    }
}