using System;
using Cysharp.Threading.Tasks;
using LogicGamer.Core;
using LogicGamer.Core.Engine.Entity;
using LogicGamer.Core.Tool;

namespace UnityLogicGamer.Utilities
{
    public static class AsyncExtensions
    {
        public static UniTask<T> ShowEntity<T>(this EntityManager manager, string group, string location, Userdata data = null) where T :class,IEntityAdapter
        {
            UniTaskCompletionSource<T> result = new UniTaskCompletionSource<T>();
           var entity= manager.ShowEntity(group,location,data);
    
           entity.AfterShow += HandleShow;
           void HandleShow(IEntityAdapter instance)
           {
               entity.AfterShow -= HandleShow;

               if (instance is T casted)
               {
                   result.TrySetResult(casted);
               }
               else
               {
                   Logic.Error($"类型转换错误: {instance?.GetType().FullName} → {typeof(T).FullName}");
                   result.TrySetException(new InvalidCastException());
               }
           }

           return result.Task;
        }
    }
}