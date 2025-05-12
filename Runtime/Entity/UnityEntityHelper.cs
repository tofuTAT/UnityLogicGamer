using System;
using System.Linq;
using LogicGamer.Core.Engine;
using LogicGamer.Core.Engine.Entity;
using LogicGamer.Core.Tool;
using LogicGamer.Core.Tool.ObjectPool;
using UnityEngine;
using UnityLogicGamer.Utilities;

namespace UnityLogicGamer.Runtime.Entity
{
    public class UnityEntityHelper : MonoBehaviour, IEntityHelper, IEngineData
    {
        [SerializeField] private EntityPool pool;

        [SerializeField] private Canvas uiRoot;
        
        [SerializeField] private GameObject entityRoot;
        // 实体对象池实例（你自己实现的单例）
        public IObjectPool<IEntity> EntityPool => pool;

        public object GetGroupRoot(string rootName)
        {
            if (rootName.StartsWith("ui", StringComparison.OrdinalIgnoreCase))
            {
                return uiRoot.transform.Find(rootName).gameObject;
            }
            else
            {
                return uiRoot.transform.Find(rootName).gameObject;
            }
        }

        // 设置实体分组行为（例如UI分组的特定处理）
        public void SetGroup(string group, IEntity entity)
        {
            var root = CreateOrGetRoot(group);
            // 处理 UI 元素，假设 entity 是 MonoBehaviour 类型
            if (entity is not UnityEntity unityObj)
            {
                // 抛出异常，指示 entity 不是 UnityEntity 类型
                throw new InvalidCastException($"无法将实体转换为 UnityEntity 类型，Location: {entity.Location}");
            }
            if (group.StartsWith("ui", StringComparison.OrdinalIgnoreCase))
            {
                var rect = unityObj.gameObject.GetOrAddComponent<RectTransform>();
                rect.sizeDelta = ((RectTransform)root.transform).sizeDelta;
            }

            var objTransform = unityObj.transform;
            objTransform.SetParent(root);
            objTransform.localPosition=Vector3.zero;
            objTransform.localScale=Vector3.one;
            unityObj.gameObject.SetActive(true);
        }

        // 根据 group 创建或根节点
        private Transform CreateOrGetRoot(string group)
        {
            Transform root;
            if (group.StartsWith("ui", StringComparison.OrdinalIgnoreCase))
            {
                 root = uiRoot.transform.Find(group);
                 
            }
            else
            {
                 root = entityRoot.transform.Find(group);
            }
            if (!root)
            {
                throw new NullReferenceException($"找不到实体分组,请提前创建好分组{group},请提前创建好分组");
            }
            return root;
        }

        // 提供给框架的类型声明，用于注册到 EntityManager
        public Type Type => typeof(EntityManager);
        // 提供初始化数据，用于 Logic.Start 的 dataMap 注入
        public Userdata Data
        {
            get
            {
                if (_data == null)
                {
                    _data = new Userdata();
                    _data.Set(EntityManager.HelperKey, this);
                }
                return _data;
                
            }
        }

        // 私有缓存数据（延迟初始化）
        private Userdata _data;
    }
}
