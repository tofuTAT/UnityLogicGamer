using System;
using System.Collections.Generic;
using HybridCLR.Editor.Commands;
using UnityEngine;
using UnityLogicGamer.Editor.Build;
using System.IO;
using System.Linq;
using System.Reflection;
using HybridCLR.Editor;
using LogicGamer.Core.Utilities;
using UnityEditor;
using UnityLogicGamer.Editor.Config;
using UnityLogicGamer.HybirdClr.Runtime;

namespace UnityLogicGamer.HybirdClr.Editor
{
    public interface IHybirdClrAotProvider
    {
        List<string> GetPatchedAOTAssemblyList();
    }

    [Serializable]
    public class HybirdClrStep : IBuildStep
    {
        [SerializeField] private string hotfixOutput = "Assets/Hotfix"; // 热更依赖的生成目录
        [SerializeField] private string hotfixConfigOutPut = "Assets/Hotfix"; // 热更配置文件的生成目录
        [SerializeField] private bool debugMode = false; // 是否处于调试模式
        [SerializeField] private bool firstPackage = false; // 是否为首次打包
        [SerializeField] private string startScenePath;

        // 生成构建步骤
        public void Build()
        {
            if (firstPackage)
            {
                // 第一次打包时，生成所有热更新元数据及相关程序集
                PrebuildCommand.GenerateAll();
            }
            else
            {
                // 生成热更新元数据，暂时不进行元数据变动的检测优化
                StripAOTDllCommand.GenerateStripedAOTDlls();
                //生成dll
                CompileDllCommand.CompileDllActiveBuildTargetRelease();
            }
            //清空目录
            Utility.Directory.EnsureAndClearDirectory(Path.Combine(UnityLogicGamerEditorConfig.ProjectRoot, hotfixOutput));
            var targetDir =Path.Combine(UnityLogicGamerEditorConfig.ProjectRoot, hotfixOutput);
            //aot
            var aotList = GetPatchedAOTAssemblyList();
            var aotDir =Path.Combine(UnityLogicGamerEditorConfig.ProjectRoot, SettingsUtil.GetAssembliesPostIl2CppStripDir(EditorUserBuildSettings.activeBuildTarget));
        
            List<TextAsset> aots = UnityLogicGamer.Editor.Util.Utilities.CopyAndGetAssets<TextAsset>(
                aotList.ToArray(),aotDir,targetDir, NameEx);

            // 热更 DLL
            var dllList = SettingsUtil.HotUpdateAssemblyFilesExcludePreserved;
            var dllDir = Path.Combine(UnityLogicGamerEditorConfig.ProjectRoot, SettingsUtil.GetHotUpdateDllsOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget)); 
            List<TextAsset> dlls = UnityLogicGamer.Editor.Util.Utilities.CopyAndGetAssets<TextAsset>(
                dllList.ToArray(),dllDir,targetDir, NameEx);

            var pdbList =SettingsUtil.HotUpdateAssemblyNamesExcludePreserved.Select(dll => dll + ".pdb").ToArray();
            // PDB（仅在 debugMode 为 true 时）
            List<TextAsset> pdbs = debugMode
                ? UnityLogicGamer.Editor.Util.Utilities.CopyAndGetAssets<TextAsset>(
                    pdbList,dllDir,targetDir, NameEx)
                : new List<TextAsset>();

            Utility.Directory.EnsureAndClearDirectory(Path.Combine(UnityLogicGamerEditorConfig.ProjectRoot, hotfixConfigOutPut));
            // 保存 HotfixConfig 实例
            var asset=  CreateHotfixConfig(aots,dlls,pdbs,startScenePath);
            AssetDatabase.CreateAsset(asset,Path.Combine(hotfixConfigOutPut, "HotFixConfig.asset"));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private string NameEx(string name)
        {
            return name + ".bytes";
        }

        private List<string> GetPatchedAOTAssemblyList()
        {

            var types = Utility.Type.GetTypesImplementing<IHybirdClrAotProvider>().ToList();
            // 如果没有找到任何实现，抛出异常
            if (types.Count == 0)
            {
                throw new InvalidOperationException("没有找到实现 IHybirdClrAotProvider 接口的类型");
            }

            // 如果找到多个实现，抛出异常
            if (types.Count > 1)
            {
                throw new InvalidOperationException("找到多个实现 IHybirdClrAotProvider 的类型，请确保只有一个实现");
            }

            IHybirdClrAotProvider provider = (IHybirdClrAotProvider)Activator.CreateInstance(types.First());
            return provider.GetPatchedAOTAssemblyList();
        }

        private HotfixConfig CreateHotfixConfig(List<TextAsset> aot,List<TextAsset> dll ,List<TextAsset> pdb ,string startScene)
        {
            // 创建 HotfixConfig 实例
            HotfixConfig hotfixConfig = ScriptableObject.CreateInstance<HotfixConfig>();
            hotfixConfig.AOTList = aot;
            hotfixConfig.DLLList = dll;
            hotfixConfig.PdbList = pdb;
            hotfixConfig.StartScenePath = startScene;
            return hotfixConfig;
        }
    }
}

