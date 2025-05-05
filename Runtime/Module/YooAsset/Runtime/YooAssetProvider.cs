#if YOO_ASSET_2_3_OR_NEWER
using System;
using LogicGamer.Core.Attributes;
using LogicGamer.Core.Engine.ResourceProvider;
using LogicGamer.Core.Tool;
using YooAsset;

namespace UnityLogicGamer.Runtime.Adapter.YooAsset
{
    [Enable] // 支持自动注册
    public class YooAssetProvider : IResourceProvider
    {
        public IResourceLoader GetLoader(string location, Userdata args)
        {
            return new YooAssetResourceLoader(location, this);
        }

        public bool CheckLocation(string location)
        {
            return YooAssets.CheckLocationValid(location);
        }

        public void Release()
        {
        }

        private class YooAssetResourceLoader : IResourceLoader
        {
            public string Location { get; }
            public IResourceProvider Provider { get; }

            private AssetHandle assetHandle;

            public YooAssetResourceLoader(string location, IResourceProvider provider)
            {
                Location = location;
                Provider = provider;
            }

            public void GetAsset<T>(Action<LoaderState, T> loadFinish)
            {
                GetAsset(typeof(T),
                    (state,obj) =>
                    {
                        loadFinish?.Invoke(state,(T)obj);
                    } );
            }

            public void GetAsset(Type type, Action<LoaderState, object> loadFinish)
            {
                if (assetHandle==null)
                {
                    assetHandle = YooAssets.LoadAssetAsync(Location, type);
                }

                assetHandle.Completed += HandleLoadFinish;
                void HandleLoadFinish(AssetHandle obj)
                {
                    assetHandle.Completed -= HandleLoadFinish;
                    loadFinish.Invoke(obj.Status == EOperationStatus.Succeed?LoaderState.Success:LoaderState.Error,obj.AssetObject);
                }
            }

            public void UnLoad()
            {
                assetHandle?.Release();
                assetHandle = null;
            }
        }
    }
}
#endif