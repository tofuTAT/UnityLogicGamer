using System;
using YooAsset.Editor;

namespace UnityLogicGamer.Runtime.Module.YooAsset.Editor.AutoImport
{
    public class RawfileBuildpipelineConfig:YooAssetBundleConfig
    {
        protected override EBuildPipeline BuildPipeline => EBuildPipeline.RawFileBuildPipeline;
        
        public override void ExecuteBuild()
        {
         
        }

    }
}