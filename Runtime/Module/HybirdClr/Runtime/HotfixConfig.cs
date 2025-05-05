using System.Collections.Generic;
using UnityEngine;

namespace UnityLogicGamer.HybirdClr.Runtime
{
    [CreateAssetMenu(menuName = "HotfixConfig")]
    public  class HotfixConfig:ScriptableObject
    {
    
        [SerializeField] private List<TextAsset> dllList;

        [SerializeField] private List<TextAsset> pdbList;
        [SerializeField] private List<TextAsset> aotList;

        [SerializeField] private string startScenePath;

        public List<TextAsset> AOTList
        {

            get => aotList;

            set => aotList = value;
        }

        public string StartScenePath
        {

            get => startScenePath;

            set => startScenePath = value;
        }

        public List<TextAsset> PdbList
        {

            get => pdbList;

            set => pdbList = value;
        }
        public List<TextAsset> DLLList
        {

            get => dllList;

            set => dllList = value;
        }
    }
}