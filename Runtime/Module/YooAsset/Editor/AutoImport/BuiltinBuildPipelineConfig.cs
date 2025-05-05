using UnityEditor;
using UnityEngine;
using UnityLogicGamer.Runtime.Module.YooAsset.Editor.AutoImport;
using YooAsset.Editor;

namespace UnityLogicGamer.Runtime.Module.YooAsset.Editor.Config
{
    public class BuiltinBuildPipelineConfig:YooAssetBundleConfig
    {
        [SerializeField]
        private ECompressOption compression;

        protected override EBuildPipeline BuildPipeline => EBuildPipeline.BuiltinBuildPipeline;

        public override void ExecuteBuild()
        {
            var buildinFileCopyParams = AssetBundleBuilderSetting.GetPackageBuildinFileCopyParams(packageName, BuildPipeline);
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuiltinBuildParameters buildParameters = new BuiltinBuildParameters();
            buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
            buildParameters.BuildPipeline = BuildPipeline.ToString();
            buildParameters.BuildTarget = buildTarget;
            buildParameters.PackageName = packageName;
            buildParameters.PackageVersion = GetVersion();
            buildParameters.EnableSharePackRule = true;
            buildParameters.VerifyBuildingResult = true;
            buildParameters.FileNameStyle = fileNameStyle;
            buildParameters.BuildinFileCopyOption = copyBuildinFileOptionField;
            buildParameters.BuildinFileCopyParams = buildinFileCopyParams;
            buildParameters.EncryptionServices = CreateEncryptionInstance();
            buildParameters.CompressOption = compression;

            BuiltinBuildPipeline pipeline = new BuiltinBuildPipeline();
            var buildResult = pipeline.Run(buildParameters, true);
        }
    }
}