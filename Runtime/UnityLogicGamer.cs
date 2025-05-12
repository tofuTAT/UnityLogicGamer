using System;
using System.Collections.Generic;
using System.Linq;
using LogicGamer.Core;
using LogicGamer.Core.Engine;
using LogicGamer.Core.Engine.Fsm;
using LogicGamer.Core.Tool.Log;
using LogicGamer.Core.Utilities;
using UnityEngine;
using UnityLogicGamer.Runtime.Debuggers;
using UnityLogicGamer.Runtime.Procedure;

namespace UnityLogicGamer.Runtime
{
    public class UnityLogicGamer : UnitySingle<UnityLogicGamer>,ILog
    {
        [SerializeField] private LogLevel logLevel;
        [SerializeField] private bool debugMode;
        [SerializeField] private List<string> procedures;
        [SerializeField] private string startProcedure;

        public static Fsm Procedure { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            Logic.Init(this);
        }


        private List<IDebug> _debugs = new List<IDebug>();
        public void Start()
        {
            var dataMap = GetComponentsInChildren<IEngineData>(true)
                .ToDictionary(e => e.Type, e => e.Data);
            Logic.Start(dataMap);
            if (debugMode)
            {
                var types =Utility.Type.GetTypesImplementing<IDebug>();
                foreach (var item in types)
                {
                    IDebug debugger;

                    // 判断该类型是否继承自 MonoBehaviour
                    if (typeof(MonoBehaviour).IsAssignableFrom(item))
                    {
                        // 如果是 MonoBehaviour 类型，则通过 Unity 的场景管理创建
                        // 假设 item 是 MonoBehaviour 的派生类，实例化时必须挂载到一个 GameObject 上
                        GameObject debugObject = new GameObject(item.Name);
                        debugObject.transform.SetParent(transform);
                        debugger = debugObject.AddComponent(item) as IDebug;
                    }
                    else
                    {
                        // 对于非 MonoBehaviour 类型，通过反射创建实例
                        debugger = Activator.CreateInstance(item) as IDebug;
                    }

                    // 检查是否成功创建了实例
                    if (debugger == null)
                    {
                        Logic.Warning($"无法创建 IDebug 类型的实例: {item.FullName}");
                    }
                    else
                    {
                        // 初始化调试器
                        debugger.Init();
                        _debugs.Add(debugger);
                    }
                }
            }
            Procedure = Logic.GetEngine<FsmManager>().CreateFsm("GameProcedure");
      
            Type start = null;
            foreach (var item in procedures)
            {
                var type = Utility.Type.Get(item);
                if (type==null)
                {
                    throw new InvalidOperationException($"无法找到类型: {item}");
                }
                ProcedureBase state = Activator.CreateInstance(type) as ProcedureBase;
                if (state == null)
                {
                    throw new InvalidCastException($"无法将类型 {item} 实例化为 ProcedureBase");
                }
                Procedure.AddState(state);
                if (item==startProcedure)
                {
                    start = type;
                }
            }
            if (start == null)
            {
                throw new InvalidOperationException($"未找到指定的启动流程: {startProcedure}");
            }
            Procedure.ChangeState(start);
        }
        
        void Update()
        {
            Logic.Update(Time.fixedDeltaTime,Time.deltaTime);
        }

        private void OnDestroy()
        {
            foreach (var item in _debugs)
            {
                item.Release();
            }
            _debugs.Clear();
            Logic.GetEngine<FsmManager>().Release(Procedure.Name);
            Logic.ShutDown();
        }

        #region log
        public LogLevel Level
        {
            get => logLevel;
            set
            {
                logLevel = value;
            }
        }

        public void Print(LogLevel level, string value)
        {
            if (logLevel < level)
                return;

            var message = $"[{level}] {value}";

            switch (level)
            {
                case LogLevel.Error:
                    Debug.LogError(message);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogLevel.Info:
                case LogLevel.Debug:
                    Debug.Log(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }

        #endregion

    }
}
