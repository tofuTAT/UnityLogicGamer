using UnityEngine;
using YooAsset.Editor;

namespace UnityLogicGamer.Runtime.Module.YooAsset.Editor.AutoImport
{
    public class ScriptableBuildPipelineConfig:YooAssetBundleConfig
    {
        protected override EBuildPipeline BuildPipeline => EBuildPipeline.ScriptableBuildPipeline;
        [SerializeField]
        private ECompressOption compression;
        public override void ExecuteBuild()
        {

        }

    }
}