using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityLogicGamer.Utilities
{
    public static class UnityUtility
    {
        /// <summary>
        /// 删除 GameObject 上 Transform 的所有子物体
        /// </summary>
        /// <param name="gameObject">当前的 GameObject</param>
        public static void RemoveAllChildren(this GameObject gameObject)
        {
            Transform transform = gameObject.transform;
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }
        }
        
        /// <summary>
        /// 删除 GameObject 上 Transform 的所有子物体
        /// </summary>
        /// <param name="transform">当前的 Transform</param>
        public static void RemoveAllChildren(this Transform transform)
        {
            // 遍历并删除所有子物体
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// 获取或添加脚本
        /// </summary>
        /// <param name="obj">当前的 obj</param>
        /// <param name="type">组件类型</param>
        public static Component GetOrAddComponent(this GameObject obj,Type type)
        {
            var component= obj.GetComponent(type);
            if (component==null)
            {
                component=  obj.AddComponent(type);
            }

            return component;
        }
        
        /// <summary>
        /// 获取或添加脚本
        /// </summary>
        /// <param name="obj">当前的 obj</param>
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            return obj.GetOrAddComponent(typeof(T)) as T;
        }
    }
}