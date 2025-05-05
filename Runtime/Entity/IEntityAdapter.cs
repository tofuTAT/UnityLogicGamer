using LogicGamer.Core.Tool;

namespace UnityLogicGamer.Runtime.Entity
{
    public interface IEntityAdapter
    {
        void OnInit();
        
        void OnShow(Userdata data);
        /// <summary>
        /// 每帧逻辑更新。
        /// </summary>
        /// <param name="logicTime">游戏逻辑时间。</param>
        /// <param name="deltaTime">距上次更新经过的时间（单位：秒）。</param>
        void OnUpdate(float logicTime, float deltaTime);

        void OnClose();
    }
}