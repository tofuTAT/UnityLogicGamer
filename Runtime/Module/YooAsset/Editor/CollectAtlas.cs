using UnityEditor;
using YooAsset.Editor;

namespace UnityLogicGamer.Runtime.Module.YooAsset.Editor
{
    [DisplayName("收集图集")]
    public class CollectAtlas:IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            var mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath);
            return mainAssetType == typeof(UnityEngine.U2D.SpriteAtlas);
        }
    }
}