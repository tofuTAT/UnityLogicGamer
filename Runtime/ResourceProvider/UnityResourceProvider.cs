using System;
using LogicGamer.Core;
using LogicGamer.Core.Attributes;
using LogicGamer.Core.Engine.ResourceProvider;
using LogicGamer.Core.Tool;
using UnityEngine;

namespace UnityLogicGamer.Runtime.ResourceProvider
{
    [Enable] // 自动注册用
    public class UnityResourceProvider : IResourceProvider
    {
        private class UnityResourceLoader : IResourceLoader
        {
            public string Location { get; }
            public IResourceProvider Provider { get; }

            private UnityEngine.Object _cachedAsset;

            public UnityResourceLoader(string location, IResourceProvider provider)
            {
                Location = location;
                Provider = provider;
            }

            public void GetAsset<T>(Action<LoaderState, T> loadFinish)
            {
                GetAsset(typeof(T), (state, result) => { loadFinish?.Invoke(state, (T)result); });
            }

            public void GetAsset(Type type, Action<LoaderState, object> loadFinish)
            {
                if (_cachedAsset == null)
                {
                    _cachedAsset = Resources.Load(Location, type);
                }

                if (_cachedAsset == null)
                {
                    Logic.Warning($"资源未成功加载: {Location}");
                }

                loadFinish?.Invoke(_cachedAsset != null ? LoaderState.Success : LoaderState.Error, _cachedAsset);
            }

            public void UnLoad()
            {
                if (_cachedAsset != null)
                {
                    Resources.UnloadAsset(_cachedAsset);
                    _cachedAsset = null;
                    Logic.Debug($"资源被卸载: {Location}");
                }
            }





        }


        public bool CheckLocation(string location)
        {
            var temp = Resources.Load(location);
            bool has = temp != null;
            Resources.UnloadAsset(temp);
            return has;
        }

        public IResourceLoader GetLoader(string location, Userdata args)
        {
            return new UnityResourceLoader(location, this);
        }

        public void Release()
        {

        }



    }

}