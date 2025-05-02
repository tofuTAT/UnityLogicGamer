using LogicGamer.Core.Engine.Fsm;
using LogicGamer.Core.Tool;

namespace UnityLogicGamer.Runtime.Procedure
{
    public abstract class ProcedureBase : IState
    {
        public virtual void OnEnter(Fsm fsm, Userdata args)
        {
            
        }

        public virtual void OnUpdate(float logicTime, float deltaTime, Fsm fsm)
        {
        }

        public virtual void OnExit(Fsm fsm)
        {
            
        }
    }
}
