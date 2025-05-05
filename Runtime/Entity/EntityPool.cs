using System.Collections.Generic;
using System.Linq;
using LogicGamer.Core;
using LogicGamer.Core.Engine.Entity;
using LogicGamer.Core.Tool;
using LogicGamer.Core.Tool.ObjectPool;
using UnityEngine;

namespace UnityLogicGamer.Runtime.Entity
{
    public class EntityPool : MonoBehaviour, IObjectPool<IEntity>
    {
        public int CurrentSize => objs.Sum((t) => t.Value?.Count ?? 0);
        private Dictionary<string, List<UnityEntity>> objs = new();
        private string FormatGroupName(string locationKey)
        {
            return $"[IEntity] {locationKey}";
        }

        public IEntity Get(Userdata data)
        {
            string locationKey = data.Get<string>(EntityManager.LocationKey);
  
            if (!objs.TryGetValue(locationKey, out var list))
            {
                list = new List<UnityEntity>();
                objs[locationKey] = list;
            }
            UnityEntity instance = list.FirstOrDefault();
            if (instance  == null)
            {
                instance = CreateNewEntity(locationKey);
            }
            instance.OnReset(data);
            return instance;
        }

        public void Return(IEntity obj)
        {
            if (obj is not  UnityEntity unityEntity)
            {
                Logic.Error($"目标类型:{typeof(UnityEntity)} 错误的类型:{obj.GetType()}");
                return;
            }
 
            if (!objs.TryGetValue(unityEntity.Location, out var list))
            {
                list = new List<UnityEntity>();
                objs[unityEntity.Location] = list;
            }
            obj.OnReturn();
            objs[unityEntity.Location].Add(unityEntity);
            unityEntity.transform.SetParent(transform);
            unityEntity.gameObject.SetActive(false);
        }

        public void Clean(int maxSize)
        {
       
        }

        private UnityEntity CreateNewEntity(string location)
        {
            var objName = FormatGroupName(location);
            var go = new GameObject(objName);
            var entity = go.AddComponent<UnityEntity>();
            entity.gameObject.SetActive(false);
            go.transform.SetParent(transform);
            return entity;
        }
    }
}
