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
        
        /// <summary>
        /// 将世界坐标转换为 UI 坐标。
        /// </summary>
        /// <param name="worldPos">要转换的世界坐标。</param>
        /// <param name="uiElement">UI 元素的 RectTransform。</param>
        /// <param name="mainCamera">用于计算世界到屏幕坐标的主要摄像机。</param>
        /// <param name="uiCamera">用于计算屏幕坐标到 UI 坐标的 UI 摄像机。</param>
        /// <returns>转换后的 UI 坐标。</returns>
        public static Vector2 WorldPosToUI(Vector3 worldPos, RectTransform uiElement, Camera mainCamera, Camera uiCamera)
        {
            // 获取世界坐标在屏幕上的位置
            Vector2 screenPosition = mainCamera.WorldToScreenPoint(worldPos);
            // 将屏幕坐标转换为 UI 坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                uiElement.parent as RectTransform,  // 使用 UI 元素的父级作为坐标系
                screenPosition,                      // 屏幕坐标
                uiCamera,                            // 渲染 UI 的摄像机
                out Vector2 localPoint               // 输出的本地坐标
            );

            return localPoint; // 返回转换后的 UI 坐标
        }
        
        public static Camera GetUICamera()
        {
           return GameObject.Find("GameManager/EntityManager/UICamera").GetComponent<Camera>();
        }
    }
}