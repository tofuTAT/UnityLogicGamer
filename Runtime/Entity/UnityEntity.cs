using System;
using LogicGamer.Core;
using LogicGamer.Core.Engine;
using LogicGamer.Core.Engine.Entity;
using LogicGamer.Core.Engine.ResourceProvider;
using LogicGamer.Core.Tool;
using UnityEngine;

namespace UnityLogicGamer.Runtime.Entity
{
    public class UnityEntity:MonoBehaviour,IEntity,IHandle<IEntity>
    {
        private IResourceLoader loader;
        public string Location { get; private set; }

        private GameObject adapterObj;
        public IEntityAdapter Adapter { get; private set; }
        public void OnReset(Userdata data)
        {
            Args = data;
            switch (State)
            {
                case HandleState.Wait:
                    if (loader==null)
                    {
                        State = HandleState.Doing;
                        Location = data.Get<string>(EntityManager.LocationKey);
                        loader = Logic.GetEngine<ResourceProviderManager>().GetLoader(Location);
                        loader.GetAsset<GameObject>(HandleOnLoadFinish);
                    }
                    break;
                case HandleState.Success:
                    Show();
                    break;
                case HandleState.Doing:
                    Logic.Warning("疑似UnityEntity一直未加载成功 Location(已做兼容处理，视情况忽略):"+Location);
                    break;
                case HandleState.Fail:
                    Logic.Error("未知的错误 Entity Location:"+Location);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleOnLoadFinish(LoaderState arg1, GameObject asset)
        {
            if (arg1 == LoaderState.Success)
            {
                Result = this;
                State = HandleState.Success;
                //实例化
                adapterObj = Instantiate(asset,transform);
                Adapter =  adapterObj.GetComponent<IEntityAdapter>();
                Adapter.OnInit();
                OnCompleted?.Invoke(this);
                if (gameObject.activeSelf)
                {
                    Show();
                }
            }
            else
            {
                State = HandleState.Fail;
                Logic.Error($"实体加载失败 Location:{Location}");
            }
        }

        private void Show()
        {
            adapterObj.SetActive(true);
            Adapter.OnShow(Args);
        }

        public void OnReturn()
        {
            Adapter?.OnClose();  
            if (adapterObj != null)
            {
                adapterObj.SetActive(false);
            }
        }
        

        public void OnUpdate(float logicTime, float deltaTime)
        {
            if (gameObject.activeSelf)
            {
                Adapter?.OnUpdate(logicTime,deltaTime);
            }
        }


        public HandleState State { get; private set; } = HandleState.Wait;
        public Userdata Args { get; private set; }
        public event Action<IHandle> OnCompleted;
        public void Release()
        {
            loader?.UnLoad();
        }

        private void OnDestroy()
        {
            Release();
        }

        public IEntity Result { get; private set; }
    }
}