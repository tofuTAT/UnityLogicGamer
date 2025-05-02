using System.Collections.Generic;
using System.Linq;
using LogicGamer.Core;
using LogicGamer.Core.Engine.Fsm;
using UnityEngine;

namespace UnityLogicGamer.Runtime.Debuggers.FsmDebug
{
    public class FsmDebugger:MonoBehaviour,IDebug
    {

        private List<FsmDebuggerAgent> _agents = new List<FsmDebuggerAgent>();

        private void Awake()
        {
            gameObject.name = "FsmDebugger";
        }

        public void Init()
        {
            var fsmManager = Logic.GetEngine<FsmManager>();
            fsmManager.OnFsmCreate += HandleFsmCreate;
            fsmManager.OnFsmRelease += HandleFsmRelease;
        }

        public void Release()
        {
             Destroy(gameObject);
        }

        private void HandleFsmRelease(Fsm fsm)
        {
            var agent = _agents.FirstOrDefault((a) => a.name == $"FsmAgent_{fsm.Name}");
            if (agent == null)
            {
                Logic.Warning("未找到要移除的 FsmDebuggerAgent ");
                return;
            }
            _agents.Remove(agent);
            Destroy(agent.gameObject);
        }

        private void HandleFsmCreate(Fsm fsm)
        {
            
            if (fsm == null || string.IsNullOrEmpty(fsm.Name))
            {
                Logic.Error("添加了一个空的fsm");
                return;
            }
            var go = new GameObject($"FsmAgent_{fsm.Name}");
            go.transform.SetParent(this.transform);
            var agent = go.AddComponent<FsmDebuggerAgent>();
            _agents.Add(agent); 
            agent.Fsm=fsm;

        }

        private void OnDestroy()
        {
            var fsmManager = Logic.GetEngine<FsmManager>();
            fsmManager.OnFsmCreate -= HandleFsmCreate;
            fsmManager.OnFsmRelease -= HandleFsmRelease;
        }
    }
}