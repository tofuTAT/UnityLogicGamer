using UnityEngine;

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
    }
}